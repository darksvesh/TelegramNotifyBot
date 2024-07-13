using System.Runtime.CompilerServices;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class TelegramBotService
{
    private readonly TelegramBotClient botClient;

    public TelegramBotService(AppSettings settings)
    {
        botClient = new TelegramBotClient(settings.TelegramBotToken);
    }

    public async Task StartBot()
    {
         
        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Bot starting: {me.Id} . And my name is {me.FirstName} .");

        using var cts = new CancellationTokenSource();

        // Настройка приёмника сообщений
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // Получать все типы обновлений
        };

        botClient.StartReceiving(
            BotHandler.HandleUpdateAsync,
            BotHandler.HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token
        );
    }
    public async Task SendMessageAsync(string chatId, string message)
    {
        await botClient.SendTextMessageAsync(new ChatId(chatId), message);
    }
}
