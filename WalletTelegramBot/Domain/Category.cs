using MNR.SDK.DataAccess.Models;

namespace WalletTelegramBot.Domain;

public class Category : IdEntity<int>
{
    public string Name { get; set; }
    public string CategoryName { get; set; }
}