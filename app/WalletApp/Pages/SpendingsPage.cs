using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace WalletApp;

public partial class SpendingsPage : ContentPage
{
	private readonly ApiService _apiService;
	private ListView _listView;
	private Entry _amountEntry;
	private Entry _commentEntry;
	private Entry _userNameEntry;

	public SpendingsPage(ApiService apiService)
	{
		_apiService = apiService;
		Title = "Траты";
		BuildUI();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadSpendings();
	}

	private async Task LoadSpendings()
	{
		try
		{
			var spendings = await _apiService.GetSpendingsAsync();
			_listView.ItemsSource = spendings;
		}
		catch (Exception ex)
		{
			await DisplayAlert("Ошибка", $"Не удалось загрузить траты: {ex.Message}", "OK");
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
			var spending = new SpendingCreateDto
			{
				Amount = -Math.Abs(amount), // Траты сохраняются как отрицательные
				Comment = _commentEntry.Text ?? string.Empty,
				UserName = _userNameEntry.Text ?? string.Empty
			};

			await _apiService.CreateSpendingAsync(spending);
			_amountEntry.Text = string.Empty;
			_commentEntry.Text = string.Empty;
			await LoadSpendings();
		}
		catch (Exception ex)
		{
			await DisplayAlert("Ошибка", $"Не удалось добавить трату: {ex.Message}", "OK");
		}
	}

	private async void OnDeleteClicked(object sender, EventArgs e)
	{
		if (sender is Button button && button.BindingContext is SpendingDto spending)
		{
			var confirm = await DisplayAlert("Подтверждение", $"Удалить трату \"{spending.Comment}\"?", "Да", "Нет");
			if (confirm)
			{
				try
				{
					await _apiService.DeleteSpendingAsync(spending.Id);
					await LoadSpendings();
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

		_userNameEntry = new Entry { Placeholder = "Имя пользователя" };
		_amountEntry = new Entry { Placeholder = "Сумма", Keyboard = Keyboard.Numeric };
		_commentEntry = new Entry { Placeholder = "Комментарий" };

		var addButton = new Button
		{
			Text = "Добавить трату",
			Command = new Command(OnAddClicked)
		};

		formLayout.Children.Add(_userNameEntry);
		formLayout.Children.Add(_amountEntry);
		formLayout.Children.Add(_commentEntry);
		formLayout.Children.Add(addButton);

		// Список трат
		_listView = new ListView
		{
			ItemTemplate = new DataTemplate(() =>
			{
				var layout = new HorizontalStackLayout { Padding = 5, Spacing = 10 };
				var commentLabel = new Label { VerticalOptions = LayoutOptions.Center };
				commentLabel.SetBinding(Label.TextProperty, nameof(SpendingDto.Comment));
				
				var amountLabel = new Label 
				{ 
					VerticalOptions = LayoutOptions.Center,
					TextColor = Colors.Red
				};
				amountLabel.SetBinding(Label.TextProperty, nameof(SpendingDto.Amount), stringFormat: "{0:C}");
				
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
