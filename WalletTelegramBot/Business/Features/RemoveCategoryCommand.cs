using MediatR;
using MNR.SDK.Commons.Models;
using WalletTelegramBot.DataAccess.Repositories;

namespace WalletTelegramBot.Business.Features;

public class RemoveCategoryCommand : IRequest<Result>
{
    public string Name { get; set; }
}

public class RemoveCategoryCommandHandler(ISpendingsRepository repository) : IRequestHandler<RemoveCategoryCommand, Result>
{
    public async Task<Result> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result.Bad("Name is required");
        
        await repository.RemoveCategory(request.Name, cancellationToken);
        return Result.Ok();
    }
}