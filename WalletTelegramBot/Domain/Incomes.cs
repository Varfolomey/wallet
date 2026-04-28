using MNR.SDK.DataAccess.Models;

namespace WalletTelegramBot.Domain;

public class Incomes : IdEntity<int>
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Comment { get; set; }
    public int TelegramMessageId { get; set; }
}
