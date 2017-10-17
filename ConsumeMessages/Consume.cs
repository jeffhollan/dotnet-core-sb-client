using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json.Linq;

namespace publish_queue_bulk
{
    class Consume
    {
        public static void Start(int concurrent_size)
        {
            var client = new HttpClient();
            var connectionString = Environment.GetEnvironmentVariable("QUEUE_CONNECTIONSTRING");
            var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME");

            var queueClient = new QueueClient(connectionString, queueName, ReceiveMode.ReceiveAndDelete);

            client.Timeout = new TimeSpan(1, 0, 0);
            try
            {
                queueClient.RegisterMessageHandler(async (m, token) =>
                {
                    try
                    {
                        // Serialize the Service Bus message to a JObject
                        JObject message = JObject.Parse(Encoding.UTF8.GetString(m.Body));

                        Console.WriteLine(message.ToString() + " -- Downloading...");
                        await client.GetAsync("http://ipv4.download.thinkbroadband.com/10MB.zip", token);
                        Console.WriteLine("..Downloaded");

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }, new MessageHandlerOptions((exception) =>
                {
                    Console.WriteLine(exception.Exception.Data);
                    return Task.CompletedTask;
                })
                {
                    //   AutoComplete = true,
                    MaxConcurrentCalls = concurrent_size
                });
            }
            finally
            {
            }
        }

    }
}