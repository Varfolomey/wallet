namespace WalletTelegramBot.API.DTOs;

public record SpendingDto(
    int Id,
    decimal Amount,
    DateTime DateTime,
    string Comment,
    string UserName
);

public record CreateSpendingDto(
    decimal Amount,
    string Comment,
    string UserName
);

public record IncomeDto(
    int Id,
    decimal Amount,
    DateTime Date,
    string Comment
);

public record CreateIncomeDto(
    decimal Amount,
    string Comment
);

public record DayReportDto(
    DateTime Date,
    decimal TotalIncome,
    decimal TotalSpent,
    decimal Balance,
    List<SpendingItemDto> Spendings,
    List<IncomeItemDto> Incomes
);

public record SpendingItemDto(
    string Comment,
    decimal Amount
);

public record IncomeItemDto(
    string Comment,
    decimal Amount
);

public record MonthReportDto(
    string Month,
    List<SpendingItemDto> Spendings
);
