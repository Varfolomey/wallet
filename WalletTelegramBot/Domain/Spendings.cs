using MNR.SDK.DataAccess.Models;
using System.ComponentModel.DataAnnotations;

namespace WalletTelegramBot.Domain;

public class Spendings : IdEntity<int>
{
    public DateTime DateTime { get; set; }
    public decimal Amount { get; set; }
    [StringLength(255)] public string Comment { get; set; }
    [StringLength(50)] public string UserName { get; set; }
    public int TelegramMessageId { get; set; }
}
