using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
//using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class Program
{
    private static ITelegramBotClient botClient;

    static async Task Main()
    {
        // Замените "YOUR_BOT_TOKEN" на токен вашего бота, полученный у BotFather
        botClient = new TelegramBotClient("7430921391:AAGDTLtiIMIijARLzWDHQlr__c88zqiPyLg");

        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

        using var cts = new CancellationTokenSource();

        // Настройка приёмника сообщений
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // Получать все типы обновлений
        };

        botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token
        );
        while(true)
        {

        }
        // Завершение работы приёмника сообщений
        cts.Cancel();
    }

    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Обработка только сообщений
        if (update.Type != UpdateType.Message)
            return;

        var message = update.Message;

        if (message.Type != MessageType.Text)
            return;

        if (message.Text.ToLower() == "/start")
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Welcome to the bot! Type /help to see available commands.",
                cancellationToken: cancellationToken
            );
        }
        else if (message.Text.ToLower() == "/help")
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Available commands:\n/start - Start the bot\n/help - Get help",
                cancellationToken: cancellationToken
            );
        }
        else
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Unknown command. Type /help to see available commands.",
                cancellationToken: cancellationToken
            );
        }
    }

    static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"An error occurred: {exception.Message}");
        return Task.CompletedTask;
    }
}
