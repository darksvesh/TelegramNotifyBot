using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TelegramNotifyBot
{
    public class Logger
    {

        public static void WriteLine(string log)
        {
            Console.WriteLine("["+System.DateTime.Now.ToString() + "]: " + log);
        }

        public static void WriteRequest(string log, HttpListenerRequest request)
        {
            Logger.WriteLine(log + " " + "Host: " + request.RemoteEndPoint.Address.ToString() + ":" + request.RemoteEndPoint.Port.ToString() + " " + request.Headers.ToString());
        }

    }
}
