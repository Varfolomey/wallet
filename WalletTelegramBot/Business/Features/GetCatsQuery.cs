using MediatR;
using MNR.SDK.Commons.Models;
using System.Text;
using WalletTelegramBot.DataAccess.Repositories;
using WalletTelegramBot.Domain;

namespace WalletTelegramBot.Business.Features;

public class GetCatsQuery : IRequest<Result<string>>
{

}

public class GetCatsQueryHandler(IIncomesRepository incomesRepo, ISpendingsRepository spendingsRepo)
    : IRequestHandler<GetCatsQuery, Result<string>>
{
    public async Task<Result<string>> Handle(GetCatsQuery request, CancellationToken token)
    {
        var lastIncome = (await incomesRepo.GetLastIncome()) ?? new Incomes { Date = DateTime.MinValue, Amount = 0 };

        var spendings = await spendingsRepo.ReadList(a => a.DateTime >= lastIncome.Date && a.Amount < 0, a => a, token: token);
        var incomes = await spendingsRepo.ReadList(a => a.DateTime >= lastIncome.Date && a.Amount > 0, a => a, token: token);

        var was = lastIncome.Amount;
        var income = incomes.list?.Sum(a => a.Amount) ?? 0;
        var spent = (spendings.list?.Sum(a => a.Amount) ?? 0) * -1;
        var cur = was + income - spent;

        var builder = new StringBuilder();
        builder.AppendLine($"<b><u>Отчёт с {lastIncome.Date:dd.MM.yyyy HH:mm}</u></b>");
        
        if (spent != 0)
        {
            builder.AppendLine($"<b>Расходы ({spent:N0}):</b>");
        
            foreach (var item in spendings.list.GroupBy(a => a.Comment)
                         .Select(a => new { a.Key, Sum = a.ToList().Sum(e => e.Amount) * -1 })
                         .OrderByDescending(a => a.Sum))
                builder.AppendLine($"{item.Sum:N0} - {item.Key}");
        }

        if (income != 0)
        {
            builder.AppendLine().AppendLine($"<b>Доходы ({income:N0}):</b>");
            foreach (var item in incomes.list.GroupBy(a => a.Comment)
                         .Select(a => new { a.Key, Sum = a.ToList().Sum(e => e.Amount) })
                         .OrderByDescending(a => a.Sum))
                builder.AppendLine($"{item.Sum:N0} - {item.Key}");
        }

        builder.AppendLine().AppendLine("<b>Должно было остаться:</b>");

        builder.Append(was.ToString("N0"));

        if (income > 0)
            builder.Append($" + {income:N0}");
        if (spent > 0)
            builder.Append($" - {spent:N0}");
        if (income + spent > 0)
            builder.Append($" = {cur:N0}");

        return Result<string>.Ok(builder.ToString());
    }
}

