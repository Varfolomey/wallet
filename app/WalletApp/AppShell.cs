using Microsoft.Maui.Controls;

namespace WalletApp;

public partial class AppShell : Shell
{
	private readonly DashboardPage _dashboardPage;
	private readonly SpendingsPage _spendingsPage;
	private readonly IncomesPage _incomesPage;
	private readonly ReportsPage _reportsPage;

	public AppShell(DashboardPage dashboardPage, SpendingsPage spendingsPage, IncomesPage incomesPage, ReportsPage reportsPage)
	{
		InitializeComponent();

		_dashboardPage = dashboardPage;
		_spendingsPage = spendingsPage;
		_incomesPage = incomesPage;
		_reportsPage = reportsPage;

		// Добавляем страницы в навигацию
		Items.Add(new FlyoutItem
		{
			Title = "Главная",
			Items = new ShellContent
			{
				Title = "Дашборд",
				Content = _dashboardPage
			}
		});

		Items.Add(new FlyoutItem
		{
			Title = "Траты",
			Items = new ShellContent
			{
				Title = "Список трат",
				Content = _spendingsPage
			}
		});

		Items.Add(new FlyoutItem
		{
			Title = "Доходы",
			Items = new ShellContent
			{
				Title = "Список доходов",
				Content = _incomesPage
			}
		});

		Items.Add(new FlyoutItem
		{
			Title = "Отчёты",
			Items = new ShellContent
			{
				Title = "Отчёты",
				Content = _reportsPage
			}
		});
	}
}
