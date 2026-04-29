using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace WalletApp;

public class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Настройка HTTP клиента для подключения к API
		builder.Services.AddHttpClient<ApiService>(client =>
		{
			client.BaseAddress = new Uri("http://localhost:5000/"); // Замените на адрес вашего сервера
		});

		// Регистрация страниц с внедрением зависимостей
		builder.Services.AddTransient<DashboardPage>();
		builder.Services.AddTransient<SpendingsPage>();
		builder.Services.AddTransient<IncomesPage>();
		builder.Services.AddTransient<ReportsPage>();


		return builder.Build();
	}
}
