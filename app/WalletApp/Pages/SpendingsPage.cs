using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;

namespace WalletApp;

public partial class SpendingsPage : ContentPage
{
    private readonly ApiService _apiService;
    private HubConnection? _hubConnection;

    private readonly ObservableCollection<SpendingsGroupItem> _groups = [];
    private readonly ObservableCollection<string> _lastLog = [];

    public SpendingsPage(ApiService apiService)
    {
        InitializeComponent();

        _apiService = apiService;

        GroupsView.ItemsSource = _groups;
        LogView.ItemsSource = _lastLog;

        FromDatePicker.Date = DateTime.Today.AddMonths(-1);
        ToDatePicker.Date = DateTime.Today;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EnsureRealtimeAsync();
        await RefreshAsync();
    }

    private async Task EnsureRealtimeAsync()
    {
        if (_hubConnection is not null)
            return;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_apiService.GetHubUrl("hubs/spendings"))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On("spendingChanged", async () =>
            await MainThread.InvokeOnMainThreadAsync(RefreshAsync));

        _hubConnection.On("incomeChanged", async () =>
            await MainThread.InvokeOnMainThreadAsync(RefreshAsync));

        await _hubConnection.StartAsync();
    }

    private async Task RefreshAsync()
    {
        var incomes = await _apiService.GetIncomesAsync();
        var lastIncomeDate = incomes
            .OrderByDescending(x => x.Date)
            .FirstOrDefault()?.Date.Date;

        var fromDate = FromDatePicker.Date;
        if (lastIncomeDate.HasValue && fromDate < lastIncomeDate.Value)
            fromDate = lastIncomeDate.Value;

        var spendings = await _apiService.GetSpendingsAsync(
            fromDate: fromDate,
            toDate: ToDatePicker.Date);

        var categories = ParseCategoryFilter(CategoriesFilterEntry.Text);
        if (categories.Count > 0)
        {
            spendings = spendings
                .Where(s => categories.Any(c =>
                    s.Comment.Contains(c, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        var grouped = spendings
            .GroupBy(s => s.Comment)
            .Select(g => new SpendingsGroupItem(g.Key, g.Sum(x => x.Amount)))
            .OrderBy(x => x.Category)
            .ToList();

        _groups.Clear();
        foreach (var group in grouped)
            _groups.Add(group);
    }

    private static List<string> ParseCategoryFilter(string? value)
    {
        return (value ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (!decimal.TryParse(AmountEntry.Text, out var amount))
            return;

        await _apiService.CreateSpendingAsync(new SpendingCreateDto
        {
            Amount = amount,
            Comment = CommentEntry.Text ?? string.Empty,
            UserName = UserNameEntry.Text ?? string.Empty
        });

        _lastLog.Insert(0, $"{DateTime.Now:t}: {CommentEntry.Text} {amount}");
        if (_lastLog.Count > 7)
            _lastLog.RemoveAt(_lastLog.Count - 1);

        AmountEntry.Text = string.Empty;
        CommentEntry.Text = string.Empty;

        await RefreshAsync();
    }

    private async void OnApplyFilterClicked(object sender, EventArgs e)
    {
        await RefreshAsync();
    }

    private sealed record SpendingsGroupItem(string Category, decimal Total);
}
