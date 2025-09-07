using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
public class WebService
{
    private readonly NotificationService _notificationService;
    private string useEncryption = "0";
    private HttpListener httpListener;
    public WebService(NotificationService notificationService, string useMessageEncryption)
    {
        _notificationService = notificationService;
        useEncryption = useMessageEncryption;
    }

    public async Task StartWebService()
    {
        httpListener = new HttpListener();
        httpListener.Prefixes.Add("http://*:5000/message/");
        httpListener.Prefixes.Add("http://*:5000/healthcheck/");
        Console.WriteLine("Starting web server on 5000 port");

        var tcpServerTask = Task.Run(() => httpListener.Start());
        var count = 0;
        while (!httpListener.IsListening && count < 10)
        {
            Console.WriteLine("Waiting server to start");
            Thread.Sleep(500);
            count++;
        }
        var requesttask = Task.Run(() => this.RunRequestHandler());
    }

    public async Task RunRequestHandler()
    {

        while(true){
            HttpListenerContext context = await httpListener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            _ = Task.Run(() => HandleRequestAsync(context, request, _notificationService, useEncryption));
            await Task.Delay(1);
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context, HttpListenerRequest request, NotificationService notificationService, string encrypted)
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
            Console.WriteLine("Healthcheck recieved.");
            context.Response.StatusCode = 200;
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
