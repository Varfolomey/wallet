using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace WalletApp;

public partial class IncomesPage : ContentPage
{
	private readonly ApiService _apiService;
	private readonly ObservableCollection<IncomeDto> _incomes = [];
	private Entry _amountEntry = null!;
	private Entry _commentEntry = null!;
	private DatePicker _datePicker = null!;
	public IncomesPage(ApiService apiService) { _apiService = apiService; Title = "Доходы"; BuildUI(); }
	protected override async void OnAppearing() { base.OnAppearing(); await LoadIncomes(); }
	private async Task LoadIncomes() { var items = await _apiService.GetIncomesAsync(); _incomes.Clear(); foreach (var i in items.OrderByDescending(x=>x.Date)) _incomes.Add(i); }
	private async void OnAddClicked(object sender, EventArgs e)
	{
		if (!decimal.TryParse(_amountEntry.Text, out var amount)) return;
		await _apiService.CreateIncomeAsync(new IncomeCreateDto { Amount = amount, Comment = $"{_datePicker.Date:yyyy-MM-dd} | {_commentEntry.Text}" });
		_amountEntry.Text = _commentEntry.Text = string.Empty;
		await LoadIncomes();
	}
	private void BuildUI()
	{
		var grid = new Grid { ColumnDefinitions = [new GridLength(1, GridUnitType.Star), new GridLength(1, GridUnitType.Star), new GridLength(2, GridUnitType.Star)] };
		grid.Add(new Label { Text = "Сумма", FontAttributes = FontAttributes.Bold }, 0, 0);
		grid.Add(new Label { Text = "Дата", FontAttributes = FontAttributes.Bold }, 1, 0);
		grid.Add(new Label { Text = "Комментарий", FontAttributes = FontAttributes.Bold }, 2, 0);
		var cv = new CollectionView { HeightRequest = 350, ItemsSource = _incomes, ItemTemplate = new DataTemplate(() => { var row = new Grid { ColumnDefinitions = grid.ColumnDefinitions, Padding = 2 }; var a = new Label(); a.SetBinding(Label.TextProperty, nameof(IncomeDto.Amount), stringFormat:"{0:C}"); var d = new Label(); d.SetBinding(Label.TextProperty, nameof(IncomeDto.Date), stringFormat:"{0:yyyy-MM-dd}"); var c = new Label(); c.SetBinding(Label.TextProperty, nameof(IncomeDto.Comment)); row.Add(a,0); row.Add(d,1); row.Add(c,2); return row; }) };
		_datePicker = new DatePicker { Date = DateTime.Today }; _amountEntry = new Entry { Placeholder = "Сумма", Keyboard = Keyboard.Numeric }; _commentEntry = new Entry { Placeholder = "Комментарий" }; var btn = new Button { Text = "Добавить строку" }; btn.Clicked += OnAddClicked;
		Content = new ScrollView { Content = new VerticalStackLayout { Padding = 12, Children = { grid, cv, _datePicker, _amountEntry, _commentEntry, btn } } };
	}
}
