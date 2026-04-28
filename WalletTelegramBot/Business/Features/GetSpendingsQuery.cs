using MediatR;
using MNR.SDK.Commons.Models;
using WalletTelegramBot.API.DTOs;
using WalletTelegramBot.DataAccess.Repositories;
using WalletTelegramBot.Domain;

namespace WalletTelegramBot.Business.Features;

public class GetSpendingsQuery : IRequest<Result<List<SpendingDto>>>
{
    public string? UserName { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetSpendingsQueryHandler(ISpendingsRepository repository) 
    : IRequestHandler<GetSpendingsQuery, Result<List<SpendingDto>>>
{
    public async Task<Result<List<SpendingDto>>> Handle(GetSpendingsQuery request, CancellationToken token)
    {
        var spendings = await repository.GetList(
            s => (string.IsNullOrEmpty(request.UserName) || s.UserName == request.UserName)
                 && (!request.FromDate.HasValue || s.DateTime >= request.FromDate.Value)
                 && (!request.ToDate.HasValue || s.DateTime <= request.ToDate.Value),
            orderBy: s => s.DateTime,
            descending: true,
            token: token);

        var dtos = spendings.list?.Select(s => new SpendingDto(
            s.Id,
            s.Amount,
            s.DateTime,
            s.Comment ?? "",
            s.UserName ?? ""
        )).ToList() ?? [];

        return Result<List<SpendingDto>>.Ok(dtos);
    }
}

public class GetIncomesQuery : IRequest<Result<List<IncomeDto>>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetIncomesQueryHandler(IIncomesRepository repository) 
    : IRequestHandler<GetIncomesQuery, Result<List<IncomeDto>>>
{
    public async Task<Result<List<IncomeDto>>> Handle(GetIncomesQuery request, CancellationToken token)
    {
        var incomes = await repository.GetList(
            i => (!request.FromDate.HasValue || i.Date >= request.FromDate.Value)
                 && (!request.ToDate.HasValue || i.Date <= request.ToDate.Value),
            orderBy: i => i.Date,
            descending: true,
            token: token);

        var dtos = incomes.list?.Select(i => new IncomeDto(
            i.Id,
            i.Amount,
            i.Date,
            i.Comment ?? ""
        )).ToList() ?? [];

        return Result<List<IncomeDto>>.Ok(dtos);
    }
}
