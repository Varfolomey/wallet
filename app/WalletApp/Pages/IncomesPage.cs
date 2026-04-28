using Microsoft.Maui.Controls;

namespace WalletApp;

public partial class IncomesPage : ContentPage
{
	private readonly ApiService _apiService;
	private ListView _listView;
	private Entry _amountEntry;
	private Entry _commentEntry;

	public IncomesPage(ApiService apiService)
	{
		_apiService = apiService;
		Title = "Доходы";
		BuildUI();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadIncomes();
	}

	private async Task LoadIncomes()
	{
		try
		{
			var incomes = await _apiService.GetIncomesAsync();
			_listView.ItemsSource = incomes;
		}
		catch (Exception ex)
		{
			await DisplayAlert("Ошибка", $"Не удалось загрузить доходы: {ex.Message}", "OK");
		}
	}

	private async void OnAddClicked(object sender, EventArgs e)
	{
		if (!decimal.TryParse(_amountEntry.Text, out var amount))
		{
			await DisplayAlert("Ошибка", "Введите корректную сумму", "OK");
			return;
		}

		try
		{
			var income = new IncomeCreateDto
			{
				Amount = Math.Abs(amount), // Доходы сохраняются как положительные
				Comment = _commentEntry.Text ?? string.Empty
			};

			await _apiService.CreateIncomeAsync(income);
			_amountEntry.Text = string.Empty;
			_commentEntry.Text = string.Empty;
			await LoadIncomes();
		}
		catch (Exception ex)
		{
			await DisplayAlert("Ошибка", $"Не удалось добавить доход: {ex.Message}", "OK");
		}
	}

	private async void OnDeleteClicked(object sender, EventArgs e)
	{
		if (sender is Button button && button.BindingContext is IncomeDto income)
		{
			var confirm = await DisplayAlert("Подтверждение", $"Удалить доход \"{income.Comment}\"?", "Да", "Нет");
			if (confirm)
			{
				try
				{
					await _apiService.DeleteIncomeAsync(income.Id);
					await LoadIncomes();
				}
				catch (Exception ex)
				{
					await DisplayAlert("Ошибка", $"Не удалось удалить: {ex.Message}", "OK");
				}
			}
		}
	}

	private void BuildUI()
	{
		var mainLayout = new VerticalStackLayout { Padding = 10, Spacing = 10 };

		// Форма добавления
		var formLayout = new VerticalStackLayout { Spacing = 10 };

		_amountEntry = new Entry { Placeholder = "Сумма", Keyboard = Keyboard.Numeric };
		_commentEntry = new Entry { Placeholder = "Комментарий" };

		var addButton = new Button
		{
			Text = "Добавить доход",
			Command = new Command(OnAddClicked)
		};

		formLayout.Children.Add(_amountEntry);
		formLayout.Children.Add(_commentEntry);
		formLayout.Children.Add(addButton);

		// Список доходов
		_listView = new ListView
		{
			ItemTemplate = new DataTemplate(() =>
			{
				var layout = new HorizontalStackLayout { Padding = 5, Spacing = 10 };
				var commentLabel = new Label { VerticalOptions = LayoutOptions.Center };
				commentLabel.SetBinding(Label.TextProperty, nameof(IncomeDto.Comment));
				
				var amountLabel = new Label 
				{ 
					VerticalOptions = LayoutOptions.Center,
					TextColor = Colors.Green
				};
				amountLabel.SetBinding(Label.TextProperty, nameof(IncomeDto.Amount), stringFormat: "{0:C}");
				
				var deleteButton = new Button 
				{ 
					Text = "🗑️",
					BackgroundColor = Colors.Transparent,
					BorderColor = Colors.Red,
					BorderWidth = 1,
					HorizontalOptions = LayoutOptions.End
				};
				deleteButton.Clicked += OnDeleteClicked;

				layout.Children.Add(commentLabel);
				layout.Children.Add(amountLabel);
				layout.Children.Add(deleteButton);

				return layout;
			})
		};

		mainLayout.Children.Add(formLayout);
		mainLayout.Children.Add(_listView);

		Content = mainLayout;
	}
}
