using MediatR;
using MNR.SDK.Commons.Models;
using WalletTelegramBot.API.DTOs;
using WalletTelegramBot.DataAccess.Repositories;

namespace WalletTelegramBot.Business.Features;

public class GetDayReportQuery(DateTime date) : IRequest<Result<DayReportDto>>
{
    public DateTime Date { get; } = date;
}

public class GetDayReportQueryHandler(ISpendingsRepository spendingsRepo, IIncomesRepository incomesRepo) 
    : IRequestHandler<GetDayReportQuery, Result<DayReportDto>>
{
    public async Task<Result<DayReportDto>> Handle(GetDayReportQuery request, CancellationToken token)
    {
        var spendingsList = await spendingsRepo.ReadList(
            s => s.DateTime >= request.Date && s.DateTime < request.Date.AddDays(1) && s.Amount < 0,
            orderBy: s => s.DateTime,
            token: token);

        var incomesList = await spendingsRepo.ReadList(
            s => s.DateTime >= request.Date && s.DateTime < request.Date.AddDays(1) && s.Amount > 0,
            orderBy: s => s.DateTime,
            token: token);

        var totalIncome = incomesList.list?.Sum(s => s.Amount) ?? 0;
        var totalSpent = (spendingsList.list?.Sum(s => s.Amount) ?? 0) * -1;
        var balance = totalIncome - totalSpent;

        var spendings = spendingsList.list?
            .GroupBy(s => s.Comment)
            .Select(g => new SpendingItemDto(g.Key ?? "", g.Sum(s => s.Amount) * -1))
            .OrderByDescending(s => s.Amount)
            .ToList() ?? [];

        var incomes = incomesList.list?
            .GroupBy(i => i.Comment)
            .Select(g => new IncomeItemDto(g.Key ?? "", g.Sum(i => i.Amount)))
            .OrderByDescending(i => i.Amount)
            .ToList() ?? [];

        var report = new DayReportDto(
            request.Date,
            totalIncome,
            totalSpent,
            balance,
            spendings,
            incomes
        );

        return Result<DayReportDto>.Ok(report);
    }
}

public class GetMonthReportQuery(int year, int month) : IRequest<Result<List<MonthReportDto>>>
{
    public int Year { get; } = year;
    public int Month { get; } = month;
}

public class GetMonthReportQueryHandler(ISpendingsRepository repository, IIncomesRepository incomesRepository) 
    : IRequestHandler<GetMonthReportQuery, Result<List<MonthReportDto>>>
{
    public async Task<Result<List<MonthReportDto>>> Handle(GetMonthReportQuery request, CancellationToken token)
    {
        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1);

        var spends = await repository.GetList(
            s => s.DateTime >= startDate && s.DateTime < endDate && s.Amount < 0,
            token: token);

        var grouped = spends.list?
            .GroupBy(s => new { s.DateTime.Year, s.DateTime.Month })
            .Select(g => new MonthReportDto(
                $"{g.Key.Month:D2}.{g.Key.Year}",
                g.GroupBy(x => x.Comment)
                    .Select(cg => new SpendingItemDto(cg.Key ?? "", cg.Sum(x => x.Amount) * -1))
                    .OrderByDescending(x => x.Amount)
                    .ToList()
            ))
            .OrderBy(m => m.Month)
            .ToList() ?? [];

        return Result<List<MonthReportDto>>.Ok(grouped);
    }
}
