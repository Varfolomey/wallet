using MediatR;
using MNR.SDK.Commons.Models;
using WalletTelegramBot.DataAccess.Repositories;
using WalletTelegramBot.Domain;

namespace WalletTelegramBot.Business.Features;

public class AddIncomeCommand : IRequest<Result>
{
    public decimal Amount { get; set; }
    public string Comment { get; set; }
    public int TelegramMessageId { get; set; }
    public string DataAreaId { get; set; }
}

public class AddIncomeCommandHandler(IIncomesRepository incomes)
    : IRequestHandler<AddIncomeCommand, Result>
{
    public async Task<Result> Handle(AddIncomeCommand request, CancellationToken token)
    {
        await incomes.AddAndSave(new Incomes
        {
            Amount = request.Amount,
            Comment = request.Comment,
            Date = DateTime.Now,
            TelegramMessageId =  request.TelegramMessageId,
            DataAreaId =  request.DataAreaId
        }, token);

        return Result.Ok();
    }
}
