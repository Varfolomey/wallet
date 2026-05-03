using Microsoft.Maui.Controls;

namespace WalletApp;

public partial class App : Application
{
	public App(AppShell appShell)
	{
		InitializeComponent();
		MainPage = appShell;
	}
}
