using Microsoft.Maui.Controls;

namespace WalletApp;

public partial class DashboardPage : ContentPage
{
	private readonly ApiService _apiService;
	private Label _balanceLabel;
	private Label _incomesLabel;
	private Label _spendingsLabel;

	public DashboardPage(ApiService apiService)
	{
		_apiService = apiService;
		Title = "Дашборд";
		BuildUI();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadDashboardData();
	}

	private async Task LoadDashboardData()
	{
		try
		{
			var today = DateTime.Today;
			var report = await _apiService.GetDayReportAsync(today);

			_balanceLabel.Text = $"Баланс за сегодня: {report.Balance:C}";
			_incomesLabel.Text = $"Доходы: {report.TotalIncomes:C}";
			_spendingsLabel.Text = $"Траты: {report.TotalSpendings:C}";
		}
		catch (Exception ex)
		{
			_balanceLabel.Text = $"Ошибка: {ex.Message}";
		}
	}

	private void BuildUI()
	{
		var stackLayout = new VerticalStackLayout
		{
			Padding = 20,
			Spacing = 20
		};

		_balanceLabel = new Label
		{
			FontSize = 24,
			FontAttributes = FontAttributes.Bold,
			HorizontalOptions = LayoutOptions.Center
		};

		_incomesLabel = new Label
		{
			FontSize = 18,
			HorizontalOptions = LayoutOptions.Center
		};

		_spendingsLabel = new Label
		{
			FontSize = 18,
			HorizontalOptions = LayoutOptions.Center
		};

		var refreshButton = new Button
		{
			Text = "Обновить",
			HorizontalOptions = LayoutOptions.Center,
			Command = new Command(async () => await LoadDashboardData())
		};

		stackLayout.Children.Add(_balanceLabel);
		stackLayout.Children.Add(_incomesLabel);
		stackLayout.Children.Add(_spendingsLabel);
		stackLayout.Children.Add(refreshButton);

		Content = stackLayout;
	}
}
