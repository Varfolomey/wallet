namespace WalletTelegramBot.Services;

public interface IReceiverService
{
    Task ReceiveAsync(CancellationToken stoppingToken);
}
