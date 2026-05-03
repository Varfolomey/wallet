using Microsoft.Maui.Controls;

namespace WalletApp;

public class AppShell : Shell
{
	private readonly DashboardPage _dashboardPage;
	private readonly SpendingsPage _spendingsPage;
	private readonly IncomesPage _incomesPage;
	private readonly ReportsPage _reportsPage;

	public AppShell(DashboardPage dashboardPage, SpendingsPage spendingsPage, IncomesPage incomesPage, ReportsPage reportsPage)
	{
		_dashboardPage = dashboardPage;
		_spendingsPage = spendingsPage;
		_incomesPage = incomesPage;
		_reportsPage = reportsPage;

		// Добавляем страницы в навигацию
		Items.Add(new FlyoutItem
		{
			Title = "Главная",
			CurrentItem =  _dashboardPage,
		});

		Items.Add(new FlyoutItem
		{
			Title = "Траты",
			CurrentItem =  _spendingsPage,
		});

		Items.Add(new FlyoutItem
		{
			Title = "Доходы",
			CurrentItem =  _incomesPage,
		});

		Items.Add(new FlyoutItem
		{
			Title = "Отчёты",
			CurrentItem =   _reportsPage,
		});
	}
}
