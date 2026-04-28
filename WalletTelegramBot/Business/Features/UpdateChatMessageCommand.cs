using MediatR;
using MNR.SDK.Commons.Models;
using WalletTelegramBot.DataAccess.Repositories;

namespace WalletTelegramBot.Business.Features;

public class UpdateChatMessageCommand : IRequest<Result>
{
    public int TelegramMessageId { get; set; }
    public decimal Amount { get; set; }
    public string MessageText { get; set; }
    public string DataAreaId { get; set; }
}

public class UpdateChatMessageCommandHandler(ISpendingsRepository spendings) : IRequestHandler<UpdateChatMessageCommand, Result>
{
    public async Task<Result> Handle(UpdateChatMessageCommand request, CancellationToken token)
    {
        var msg = await spendings.Get(a => 
            a.DataAreaId == request.DataAreaId && 
            a.TelegramMessageId == request.TelegramMessageId, token);

        if (msg is null)
            return Result.Failed("No message found");

        msg.Comment = request.MessageText;
        msg.Amount = request.Amount;
        
        await spendings.UpdateAndSave(msg, token);
        return Result.Ok();
    }
}