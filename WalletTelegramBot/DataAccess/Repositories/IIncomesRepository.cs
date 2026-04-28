using MNR.SDK.DataAccess.Repositories;
using WalletTelegramBot.Domain;

namespace WalletTelegramBot.DataAccess.Repositories;

public interface IIncomesRepository : IBaseRepository<Incomes, int>
{
    public Task<Incomes> GetLastIncome();
}
