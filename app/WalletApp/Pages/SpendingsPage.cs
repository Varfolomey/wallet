using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;

namespace WalletApp;

public partial class SpendingsPage : ContentPage
{
	private readonly ApiService _apiService;
	private HubConnection? _hubConnection;
	private readonly ObservableCollection<SpendingsGroupItem> _groups = [];
	private readonly ObservableCollection<string> _lastLog = [];
	private CollectionView _groupsView = null!;
	private CollectionView _logView = null!;
	private Entry _amountEntry = null!;
	private Entry _commentEntry = null!;
	private Entry _userNameEntry = null!;
	private Entry _categoriesFilterEntry = null!;
	private DatePicker _fromDatePicker = null!;
	private DatePicker _toDatePicker = null!;

	public SpendingsPage(ApiService apiService) { _apiService = apiService; Title = "Главная"; BuildUI(); }
	protected override async void OnAppearing() { base.OnAppearing(); await EnsureRealtimeAsync(); await RefreshAsync(); }

	private async Task EnsureRealtimeAsync()
	{
		if (_hubConnection is not null) return;
		_hubConnection = new HubConnectionBuilder().WithUrl(_apiService.GetHubUrl("hubs/spendings")).WithAutomaticReconnect().Build();
		_hubConnection.On("spendingChanged", async () => await MainThread.InvokeOnMainThreadAsync(RefreshAsync));
		_hubConnection.On("incomeChanged", async () => await MainThread.InvokeOnMainThreadAsync(RefreshAsync));
		await _hubConnection.StartAsync();
	}

	private async Task RefreshAsync()
	{
		var incomes = await _apiService.GetIncomesAsync();
		var lastIncomeDate = incomes.OrderByDescending(x => x.Date).FirstOrDefault()?.Date.Date;
		var from = _fromDatePicker.Date;
		if (lastIncomeDate.HasValue && from < lastIncomeDate.Value) from = lastIncomeDate.Value;
		var spendings = await _apiService.GetSpendingsAsync(fromDate: from, toDate: _toDatePicker.Date);
		var cats = (_categoriesFilterEntry.Text ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (cats.Length > 0) spendings = spendings.Where(s => cats.Any(c => s.Comment.Contains(c, StringComparison.OrdinalIgnoreCase))).ToList();
		var grouped = spendings.GroupBy(s => s.Comment).Select(g => new SpendingsGroupItem(g.Key, g.Sum(x => x.Amount))).OrderBy(x => x.Category).ToList();
		_groups.Clear(); foreach (var item in grouped) _groups.Add(item);
	}

	private async void OnAddClicked(object s, EventArgs e)
	{
		if (!decimal.TryParse(_amountEntry.Text, out var amount)) return;
		await _apiService.CreateSpendingAsync(new SpendingCreateDto { Amount = amount, Comment = _commentEntry.Text ?? "", UserName = _userNameEntry.Text ?? "" });
		_lastLog.Insert(0, $"{DateTime.Now:t}: {_commentEntry.Text} {amount}"); if (_lastLog.Count > 7) _lastLog.RemoveAt(_lastLog.Count - 1);
		_amountEntry.Text = _commentEntry.Text = string.Empty;
		await RefreshAsync();
	}

	private void BuildUI()
	{
		_fromDatePicker = new DatePicker { Date = DateTime.Today.AddMonths(-1) }; _toDatePicker = new DatePicker { Date = DateTime.Today };
		_categoriesFilterEntry = new Entry { Placeholder = "Фильтр категорий (еда;транспорт)" };
		_groupsView = new CollectionView { ItemsSource = _groups, ItemTemplate = new DataTemplate(() => { var g = new Grid { ColumnDefinitions = [new(), new()], Padding = 4 }; var c = new Label(); c.SetBinding(Label.TextProperty, nameof(SpendingsGroupItem.Category)); var a = new Label { HorizontalTextAlignment = TextAlignment.End, TextColor = Colors.Red }; a.SetBinding(Label.TextProperty, nameof(SpendingsGroupItem.Total), stringFormat: "{0:C}"); g.Add(c); g.Add(a,1); return g; }) };
		_logView = new CollectionView { HeightRequest = 120, ItemsSource = _lastLog };
		_userNameEntry = new Entry { Placeholder = "Пользователь" }; _amountEntry = new Entry { Placeholder = "Сумма (+/-)", Keyboard = Keyboard.Numeric }; _commentEntry = new Entry { Placeholder = "Категория" };
		var btn = new Button { Text = "Добавить" }; btn.Clicked += OnAddClicked;
		var filterBtn = new Button { Text = "Применить фильтр" }; filterBtn.Clicked += async (_, _) => await RefreshAsync();
		Content = new ScrollView { Content = new VerticalStackLayout { Padding = 12, Spacing = 8, Children = { new Label{Text="Траты по категориям", FontAttributes=FontAttributes.Bold}, _groupsView, new Label{Text="Последние вводы"}, _logView, _fromDatePicker, _toDatePicker, _categoriesFilterEntry, filterBtn, _userNameEntry, _amountEntry, _commentEntry, btn } } };
	}

	private record SpendingsGroupItem(string Category, decimal Total);
}
