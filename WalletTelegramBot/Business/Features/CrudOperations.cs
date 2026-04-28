using MediatR;
using MNR.SDK.Commons.Models;
using WalletTelegramBot.DataAccess.Repositories;
using WalletTelegramBot.Domain;

namespace WalletTelegramBot.Business.Features;

public class CreateSpendingCommand : IRequest<Result<int>>
{
    public decimal Amount { get; set; }
    public string Comment { get; set; } = "";
    public string UserName { get; set; } = "";
}

public class CreateSpendingCommandHandler(ISpendingsRepository spendings) 
    : IRequestHandler<CreateSpendingCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateSpendingCommand request, CancellationToken token)
    {
        var spending = new Spendings
        {
            Amount = -Math.Abs(request.Amount), // Траты всегда отрицательные
            Comment = request.Comment,
            UserName = request.UserName,
            DateTime = DateTime.Now,
        };

        spending = await spendings.AddAndSave(spending, token);
        return spending != null ? Result<int>.Ok(spending.Id) : Result<int>.Failed("Не удалось сохранить трату");
    }
}

public class CreateIncomeCommand : IRequest<Result<int>>
{
    public decimal Amount { get; set; }
    public string Comment { get; set; } = "";
}

public class CreateIncomeCommandHandler(IIncomesRepository incomes)
    : IRequestHandler<CreateIncomeCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateIncomeCommand request, CancellationToken token)
    {
        var income = new Incomes
        {
            Amount = Math.Abs(request.Amount), // Доходы всегда положительные
            Comment = request.Comment,
            Date = DateTime.Now,
        };

        income = await incomes.AddAndSave(income, token);
        return income != null ? Result<int>.Ok(income.Id) : Result<int>.Failed("Не удалось сохранить доход");
    }
}

public class DeleteSpendingCommand(int id) : IRequest<Result<bool>>
{
    public int Id { get; } = id;
}

public class DeleteSpendingCommandHandler(ISpendingsRepository spendings) 
    : IRequestHandler<DeleteSpendingCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteSpendingCommand request, CancellationToken token)
    {
        var spending = await spendings.GetById(request.Id, token);
        if (spending == null)
            return Result<bool>.Failed("Трата не найдена");

        await spendings.Delete(spending, token);
        await spendings.UnitOfWork.SaveChangesAsync(token);
        return Result<bool>.Ok(true);
    }
}

public class DeleteIncomeCommand(int id) : IRequest<Result<bool>>
{
    public int Id { get; } = id;
}

public class DeleteIncomeCommandHandler(IIncomesRepository incomes)
    : IRequestHandler<DeleteIncomeCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteIncomeCommand request, CancellationToken token)
    {
        var income = await incomes.GetById(request.Id, token);
        if (income == null)
            return Result<bool>.Failed("Доход не найден");

        await incomes.Delete(income, token);
        await incomes.UnitOfWork.SaveChangesAsync(token);
        return Result<bool>.Ok(true);
    }
}
