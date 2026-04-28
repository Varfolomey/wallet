using MNR.SDK.DataAccess.Repositories;
using WalletTelegramBot.Domain;

namespace WalletTelegramBot.DataAccess.Repositories;

public interface ISpendingsRepository : IBaseRepository<Spendings, int>
{
    public Task<List<Category>> GetCategories(CancellationToken token = default);
    public Task AddCategory(Category cat, CancellationToken token = default);
    public Task RemoveCategory(string name, CancellationToken token = default);
}
