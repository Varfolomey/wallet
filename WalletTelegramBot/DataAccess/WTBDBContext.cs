using Microsoft.EntityFrameworkCore;
using MNR.SDK.DataAccess.Context;
using WalletTelegramBot.Domain;

namespace WalletTelegramBot.DataAccess;

public class WTBDBContext(DbContextOptions<WTBDBContext> options)
    : BaseContext(options)
{
    public DbSet<Incomes> Incomes { get; set; }
    public DbSet<Spendings> Spendings { get; set; }
    public DbSet<Category> Categories { get; set; }
}
