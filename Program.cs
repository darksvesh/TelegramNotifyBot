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
        
        var useMessageEncryption = Environment.GetEnvironmentVariable("TELEGRAM_BOT_USE_ENCRYPTION");

        TelegramBotService bot = new TelegramBotService(settings);
        
        NotificationService notificationService = new NotificationService(bot);
        
        await bot.StartBot();
        
        WebService web = new WebService(notificationService, useMessageEncryption);
        
        await web.StartWebService();
        
        web.RunRequestHandler();

        Console.WriteLine("Bot running...");

        while(true)
        {

        }
    }


}
