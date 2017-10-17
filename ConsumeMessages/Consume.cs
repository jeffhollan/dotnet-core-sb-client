using System;
using System.Collections.Concurrent;
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
            var connectionString = Environment.GetEnvironmentVariable("QUEUE_CONNECTIONSTRING");
            var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME");

            var queueClient = new QueueClient(connectionString, queueName, ReceiveMode.ReceiveAndDelete);
            try
            {
                queueClient.RegisterMessageHandler((m, token) =>
                {
                    // Serialize the Service Bus message to a JObject
                    JObject message = JObject.Parse(Encoding.UTF8.GetString(m.Body));
                    
                    Console.WriteLine(message.ToString());

                    // Reverse the GUID property 1000 times
                    // for(int y = 0; y < 10000; y++) {
                    //     Array.Reverse(((string)message["id"]).ToCharArray());
                    // }

                    Thread.Sleep(2000);

                    return Task.CompletedTask;
                }, new MessageHandlerOptions((exception) =>
                {
                    Console.WriteLine(exception.Exception.Data);
                    return Task.CompletedTask;
                })
                {
                    AutoComplete = true,
                    MaxConcurrentCalls = concurrent_size
                });
            }
            finally
            {
            }
        }

    }
}