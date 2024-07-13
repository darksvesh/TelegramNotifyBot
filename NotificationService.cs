public class NotificationService
{
    private readonly TelegramBotService _telegramBotService;
    public NotificationService(TelegramBotService telegramBotService)
    {
        _telegramBotService = telegramBotService;
    }

    public async Task SendNotificationAsyncEncrypted(Notification notify)
    {
        try{
            var chatId = EncryptionService.Decrypt(notify.EncryptedChatId);
            var message = EncryptionService.Decrypt(notify.EncryptedMessage);
            var checksum = EncryptionService.Decrypt(notify.EncryptedChecksum);
            var datetime = EncryptionService.Decrypt(notify.EncryptedDateTime);
            var messageUTS = long.Parse(datetime);
            var localUTS = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var offset = 5;
            if(checksum.Equals(chatId + message + datetime)){
                if(messageUTS > localUTS + offset || messageUTS < localUTS - offset )
                await _telegramBotService.SendMessageAsync(chatId, message);
            }
        }
        catch (Exception err) {
            Console.WriteLine("Caught wrongly encrypted message.");
        }
    }
    public async Task SendNotificationAsync(Notification notify)
    {
        var chatId = EncryptionService.Decrypt(notify.EncryptedChatId);
        var message = notify.EncryptedMessage;
        await _telegramBotService.SendMessageAsync(chatId, message);
    }
}