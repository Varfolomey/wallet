using Microsoft.EntityFrameworkCore;
using MNR.SDK.Commons.MediatR;
using Telegram.Bot;
using WalletTelegramBot.DataAccess;
using WalletTelegramBot.DataAccess.Repositories;
using WalletTelegramBot.DataAccess.Repositories.Internals;
using WalletTelegramBot.Extensions;
using WalletTelegramBot.Services.Internals;

namespace WalletTelegramBot;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var services = builder.Services;

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddMediatr(typeof(Program));

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddHttpClient("telegram_bot_client").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>(
            httpClient => new TelegramBotClient("7458035994:AAGbmV_daQYrXSeA3TtlAQtFtVX9LFjdre0", httpClient));
        services.AddScoped<UpdateHandler>();
        services.AddScoped<ReceiverService>();
        services.AddHostedService<PollingService>();


        services.AddDbContext<WTBDBContext>(opts => opts.UseNpgsql(
            builder.Configuration.GetConnectionString("default")));
        services.AddScoped<IIncomesRepository, IncomesRepository>();
        services.AddScoped<ISpendingsRepository, SpendingsRepository>();

        var app = builder.Build();
        app.Migrate();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
