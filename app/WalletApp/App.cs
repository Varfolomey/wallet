using Microsoft.Maui.Controls;

namespace WalletApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
}
