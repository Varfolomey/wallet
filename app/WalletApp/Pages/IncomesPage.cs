using System.Collections.ObjectModel;

namespace WalletApp;

public partial class IncomesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly ObservableCollection<IncomeDto> _incomes = [];

    public IncomesPage(ApiService apiService)
    {
        InitializeComponent();

        _apiService = apiService;
        IncomesView.ItemsSource = _incomes;
        IncomeDatePicker.Date = DateTime.Today;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadIncomesAsync();
    }

    private async Task LoadIncomesAsync()
    {
        var items = await _apiService.GetIncomesAsync();

        _incomes.Clear();
        foreach (var income in items.OrderByDescending(x => x.Date))
            _incomes.Add(income);
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (!decimal.TryParse(AmountEntry.Text, out var amount))
            return;

        await _apiService.CreateIncomeAsync(new IncomeCreateDto
        {
            Amount = amount,
            Comment = $"{IncomeDatePicker.Date:yyyy-MM-dd} | {CommentEntry.Text}"
        });

        AmountEntry.Text = string.Empty;
        CommentEntry.Text = string.Empty;

        await LoadIncomesAsync();
    }
}
