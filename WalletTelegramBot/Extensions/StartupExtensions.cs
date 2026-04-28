using Microsoft.EntityFrameworkCore;
using WalletTelegramBot.DataAccess;

namespace WalletTelegramBot.Extensions;

public static class StartupExtensions
{
    public static IApplicationBuilder Migrate(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WTBDBContext>();
        context.Database.Migrate();

        return builder;
    }
}
