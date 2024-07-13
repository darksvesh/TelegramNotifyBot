
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class BotHandler {
    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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
                text: "Welcome to the Notification Bot! Type /help to see available commands.",
                cancellationToken: cancellationToken
            );
        }
        else if (message.Text.ToLower() == "/help")
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Available commands:\n/start - Start the bot\n/help - Get help\n/token - Get you chat token.\nThis bot sends notifications via Telegram.\nSend notifications to bot via http.\n",
                cancellationToken: cancellationToken
            );
        }
        else if (message.Text.ToLower() == "/token")
        {
            string token = EncryptionService.Encrypt(message.Chat.Id.ToString());
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Your token: {token}",
                cancellationToken: cancellationToken
            );
        }
        else
        {
            //string token = EncryptionService.Decrypt(message.Text);

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Unknown command. Type /help to see available commands.",
                cancellationToken: cancellationToken
            );
        }
    }

    public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"An error occurred: {exception.Message}");
        return Task.CompletedTask;
    }

}