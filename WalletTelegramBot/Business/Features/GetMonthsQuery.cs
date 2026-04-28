using System.Globalization;
using System.Text;
using MediatR;
using MNR.SDK.Commons.Models;
using WalletTelegramBot.DataAccess.Repositories;
using WalletTelegramBot.DataAccess.Repositories.Internals;

namespace WalletTelegramBot.Business.Features;

public class GetMonthsQuery : IRequest<Result<string>>
{
    
}

public class GetMonthsQueryHandler(ISpendingsRepository repository
, IIncomesRepository incomesRepository) : IRequestHandler<GetMonthsQuery, Result<string>>
{
    public async Task<Result<string>> Handle(GetMonthsQuery request, CancellationToken cancellationToken)
    {
        var cats = await repository.GetCategories(cancellationToken);

        var spends = await repository.GetList(a => cats
            .Select(d => d.Name.Trim())
            .Contains(a.Comment.Trim()), cancellationToken);
        var periods = await incomesRepository.GetList(a => a.Id > 0, cancellationToken);
        
        var grouped = spends.GroupBy(d => 
            DateTime.ParseExact(d.DateTime.ToString("01.MM.yyyy"), "dd.MM.yyyy", CultureInfo.InvariantCulture));
        
        var builder = new StringBuilder();
        builder.AppendLine("<b><u>Отчёт по месяцам</u></b>");
        
        foreach (var spend in grouped.OrderBy(i => i.Key))
        {
            var date = spend.Key;
            builder.AppendLine($"<b><u>{date:MM.yyyy}:</u></b>");
            var data = spend.ToList().GroupBy(a => a.Comment.Trim());

            foreach (var g in data)
            {
                var k = g.Key;
                var sum = decimal.Round(g.Sum(a => a.Amount) * -1, 0);
                builder.AppendLine($"{k} - {sum}");
            }
        }
        
        return Result<string>.Ok(builder.ToString());
    }
}