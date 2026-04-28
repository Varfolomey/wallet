using Microsoft.EntityFrameworkCore;
using MNR.SDK.DataAccess.Repositories.Internals;
using WalletTelegramBot.Domain;

namespace WalletTelegramBot.DataAccess.Repositories.Internals;

internal class SpendingsRepository(WTBDBContext context)
    : BaseRepository<Spendings, int>(context), ISpendingsRepository
{
    protected override DbSet<Spendings> Set => context.Spendings;
    
    public Task<List<Category>> GetCategories(CancellationToken token = default)
        => context.Categories.ToListAsync(token);

    public async Task AddCategory(Category cat, CancellationToken token = default)
    {
        context.Categories.Add(cat);
        await context.SaveChangesAsync(token);
    }

    public async Task RemoveCategory(string name, CancellationToken token = default)
    {
        var cats = await context.Categories.Where(a => a.Name == name).ToListAsync(token);
        context.Categories.RemoveRange(cats);
        await context.SaveChangesAsync(token);
    }
}
