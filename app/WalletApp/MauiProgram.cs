using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace WalletApp;

public class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
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

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
