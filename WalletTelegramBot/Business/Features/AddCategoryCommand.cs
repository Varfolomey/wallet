using MediatR;
using MNR.SDK.Commons.Models;
using WalletTelegramBot.DataAccess.Repositories;
using WalletTelegramBot.Domain;

namespace WalletTelegramBot.Business.Features;

public class AddCategoryCommand : IRequest<Result>
{
    public string Name { get; set; }
    public string CategoryName { get; set; }
}

public class AddCategoryCommandHandler(ISpendingsRepository spendingsRepository) : IRequestHandler<AddCategoryCommand, Result>
{
    public async Task<Result> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.CategoryName))
            return Result.Bad("Name and Category are required");
        
        await spendingsRepository.AddCategory(
            new Category {Name = request.Name, CategoryName = request.CategoryName}, cancellationToken);
        
        return Result.Ok();
    }
}