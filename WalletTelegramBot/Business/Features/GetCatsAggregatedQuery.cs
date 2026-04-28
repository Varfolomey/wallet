using System.Text;
using MediatR;
using MNR.SDK.Commons.Models;
using WalletTelegramBot.DataAccess.Repositories;
using WalletTelegramBot.Domain;

namespace WalletTelegramBot.Business.Features;

public class GetCatsAggregatedQuery : IRequest<Result<string>>
{
    
}

public class GetCatsAggregatedQueryHandler(IIncomesRepository incomesRepo, ISpendingsRepository spendingsRepo) 
    : IRequestHandler<GetCatsAggregatedQuery, Result<string>>
{
    public async Task<Result<string>> Handle(GetCatsAggregatedQuery request, CancellationToken token)
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

        var grouped = (spendings.list ?? [])
            .Concat(incomes.list ?? [])
            .GroupBy(a => a.Comment)
            .Select(a => new { comment = a.Key, amount = a.ToList().Sum(e => e.Amount) * -1})
            .Where(a => a.amount != 0 && !string.IsNullOrWhiteSpace(a.comment))
            .OrderByDescending(a => a.amount)
            .ToList();
        
        builder.AppendLine($"<b>Расходы ({spent:N0}):</b>");
        foreach (var group in grouped)
            builder.AppendLine($"{group.amount:N0} - {group.comment}");

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