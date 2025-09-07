using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

class Program
{

    static async Task Main()
    {
        AppSettings settings = new AppSettings();
        
        settings.TelegramBotToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
        
        settings.UseMessageEncryption = Environment.GetEnvironmentVariable("TELEGRAM_BOT_USE_ENCRYPTION");

        settings.EncryptionSecret = Environment.GetEnvironmentVariable("TELEGRAM_BOT_ENCRYPTION_SECRET");

        TelegramBotService bot = new TelegramBotService(settings);

        NotificationService notificationService = new NotificationService(bot);

        EncryptionService.SetEncryptionKey(settings.EncryptionSecret);

        await bot.StartBot();
        
        WebService web = new WebService(notificationService, settings.UseMessageEncryption);

        web.StartWebService();

        Console.WriteLine("Awaiting connections...");

        while(true)
        {
            Thread.Sleep(10000);
        }
    }


}
