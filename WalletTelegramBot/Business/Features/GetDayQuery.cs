using System.Text;
using MediatR;
using MNR.SDK.Commons.Models;
using WalletTelegramBot.DataAccess.Repositories;

namespace WalletTelegramBot.Business.Features;

public class GetDayQuery(DateTime date) : IRequest<Result<string>>
{
    public DateTime Date { get; } = date;
}

public class GetDayQueryHandler(ISpendingsRepository spendingsRepo) : IRequestHandler<GetDayQuery, Result<string>>
{
    public async Task<Result<string>> Handle(GetDayQuery request, CancellationToken token)
    {
        var spendings = await spendingsRepo.ReadList(a => a.DateTime >= request.Date && 
                                                          a.DateTime < request.Date.AddDays(1) && 
                                                          a.Amount < 0, a => a, token: token);
        var incomes = await spendingsRepo.ReadList(a => a.DateTime >= request.Date && 
                                                        a.DateTime < request.Date.AddDays(1) && 
                                                        a.Amount > 0, a => a, token: token);
        
        var income = incomes.list?.Sum(a => a.Amount) ?? 0;
        var spent = (spendings.list?.Sum(a => a.Amount) ?? 0) * -1;
        
        var builder = new StringBuilder();
        builder.AppendLine($"<b><u>Отчёт за {request.Date:dd.MM.yyyy}</u></b>");

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
        
        builder.AppendLine().AppendLine("<b>Итого за день:</b>");

        if (income > 0)
            builder.Append($"{income:N0}");
        if (spent > 0)
            builder.Append($" - {spent:N0}");
        if (income > 0 && spent > 0)
            builder.Append($" = {(income - spent):N0}");

        return Result<string>.Ok(builder.ToString());
    }
}