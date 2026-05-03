namespace WalletApp;

public partial class ReportsPage : ContentPage
{
	private readonly ApiService _apiService;
	private Picker _monthPicker;
	private Picker _yearPicker;
	private Label _resultLabel;

	public ReportsPage(ApiService apiService)
	{
		_apiService = apiService;
		Title = "Отчёты";
		BuildUI();
	}

	private async void OnLoadReportClicked(object sender, EventArgs e)
	{
		if (_monthPicker.SelectedItem is not int month || _yearPicker.SelectedItem is not int year)
		{
			await DisplayAlert("Ошибка", "Выберите месяц и год", "OK");
			return;
		}

		try
		{
			var report = await _apiService.GetMonthReportAsync(year, month);
			
			var sb = new System.Text.StringBuilder();
			sb.AppendLine($"Отчёт за {month:00}.{year}");
			sb.AppendLine("------------------------");
			sb.AppendLine($"Итоговый баланс: {report.TotalBalance:C}");
			sb.AppendLine();
			
			foreach (var day in report.Days)
			{
				sb.AppendLine($"{day.Date:dd.MM}: Доходы={day.TotalIncomes:C}, Траты={day.TotalSpendings:C}, Баланс={day.Balance:C}");
			}

			_resultLabel.Text = sb.ToString();
		}
		catch (Exception ex)
		{
			_resultLabel.Text = $"Ошибка: {ex.Message}";
		}
	}

	private void BuildUI()
	{
		var mainLayout = new VerticalStackLayout { Padding = 10, Spacing = 15 };

		// Выбор периода
		var periodLayout = new HorizontalStackLayout { Spacing = 10 };

		_monthPicker = new Picker { Title = "Месяц" };
		for (int i = 1; i <= 12; i++)
			_monthPicker.Items.Add(i.ToString());

		_yearPicker = new Picker { Title = "Год" };
		int currentYear = DateTime.Now.Year;
		for (int i = currentYear - 2; i <= currentYear + 1; i++)
			_yearPicker.Items.Add(i.ToString());

		// Выбрать текущий месяц и год по умолчанию
		_monthPicker.SelectedIndex = DateTime.Now.Month - 1;
		_yearPicker.SelectedIndex = 2; // currentYear - (currentYear - 2) = 2

		var loadButton = new Button
		{
			Text = "Загрузить отчёт",
			//Command = new Command(OnLoadReportClicked)
		};

		periodLayout.Children.Add(_monthPicker);
		periodLayout.Children.Add(_yearPicker);

		// Результат
		_resultLabel = new Label
		{
			FontSize = 14,
			LineBreakMode = LineBreakMode.WordWrap
		};

		mainLayout.Children.Add(periodLayout);
		mainLayout.Children.Add(loadButton);
		mainLayout.Children.Add(_resultLabel);

		Content = mainLayout;
	}
}
