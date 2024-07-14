using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;

public class WebService
{
    private readonly NotificationService _notificationService;
    private string useEncryption = "0";
    private HttpListener httpListener;

    private Cancellation token;
    public WebService(NotificationService notificationService, string useMessageEncryption)
    {
        _notificationService = notificationService;
        useEncryption = useMessageEncryption;
        var cts = new CancellationTokenSource();
        token = cts.Token;
    }

    public async Task StartWebService()
    {
        httpListener = new HttpListener();
        httpListener.Prefixes.Add("http://*:5000/message/");
        httpListener.Prefixes.Add("http://*:5000/healthcheck/");
        var tcpServerTask = Task.Run(() => httpListener.Start(token));
    }

    public async Task RunRequestHandler()
    {

        while(true){
            if (tcpListener.Pending())
            {
                HttpListenerContext context = await httpListener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                _ = Task.Run(() => HandleRequestAsync(context, request, _notificationService, useEncryption));
            }
            else
            {
                await Task.Delay(10, token); // Добавление асинхронной задержки для снижения нагрузки на CPU
            }
        }
    }

    private static async Task HandleRequestAsync(HttpListenerContext context, HttpListenerRequest request, NotificationService notificationService, string encrypted)
    {
        if (request.Url.AbsolutePath == "/message")
        {
            try
            {
                using var reader = new System.IO.StreamReader(context.Request.InputStream);
                var requestBody = await reader.ReadToEndAsync();
                Notification notification = JsonSerializer.Deserialize<Notification>(requestBody);
                if (notification != null)
                {
                    
                    if(encrypted == "1")
                    {
                        await notificationService.SendNotificationAsyncEncrypted(notification);
                    } 
                    else
                    {
                        await notificationService.SendNotificationAsync(notification);
                    }
                    context.Response.StatusCode = 200;
                    await context.Response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes("Notification sent."));
                }
                else
                {
                    context.Response.StatusCode = 400;
                    await context.Response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes("Invalid request."));
                }
            }
            catch(Exception err)
            {
                Console.WriteLine(err.Message);    
            }
        }
        else if(request.Url.AbsolutePath == "/healthcheck")
        {
            await context.Response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes("OK"));
        }
        else
        {
            context.Response.StatusCode = 405;
            await context.Response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes("Method not allowed."));
        }

        context.Response.Close();
    }
}
