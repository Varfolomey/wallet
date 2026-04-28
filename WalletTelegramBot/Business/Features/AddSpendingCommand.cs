using MediatR;
using MNR.SDK.Commons.Models;
using WalletTelegramBot.DataAccess.Repositories;
using WalletTelegramBot.Domain;

namespace WalletTelegramBot.Business.Features;

public class AddSpendingCommand : IRequest<Result>
{
    public decimal Amount { get; set; }
    public string MessageText { get; set; }
    public string Author { get; internal set; }
    public int TelegramMessageId { get; set; }
    public string DataAreaId { get; set; }
}

public class AddSpendingCommandHandler(ISpendingsRepository spendings) : IRequestHandler<AddSpendingCommand, Result>
{
    public async Task<Result> Handle(AddSpendingCommand request, CancellationToken token)
    {
        var spending = new Spendings
        {
            Amount = request.Amount,
            Comment = request.MessageText,
            UserName = request.Author,
            DateTime = DateTime.Now,
            TelegramMessageId = request.TelegramMessageId,
            DataAreaId = request.DataAreaId,
        };

        spending = await spendings.AddAndSave(spending, token);

        return spending is { } ? Result.Ok() : Result.Failed("");
    }
}
