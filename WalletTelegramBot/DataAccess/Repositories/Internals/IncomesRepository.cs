using Microsoft.EntityFrameworkCore;
using MNR.SDK.DataAccess.Repositories.Internals;
using WalletTelegramBot.Domain;

namespace WalletTelegramBot.DataAccess.Repositories.Internals;

internal class IncomesRepository(WTBDBContext context)
    : BaseRepository<Incomes, int>(context), IIncomesRepository
{
    protected override DbSet<Incomes> Set => context.Incomes;

    public Task<Incomes> GetLastIncome()
        => Set.OrderByDescending(a => a.Date).FirstOrDefaultAsync();

}
