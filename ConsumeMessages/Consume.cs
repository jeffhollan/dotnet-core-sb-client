using System;
using System.Collections.Concurrent;
using System.IO;
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
            Random rng = new Random();
            var connectionString = Environment.GetEnvironmentVariable("QUEUE_CONNECTIONSTRING");
            var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME");
            int fileSize = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("FILESIZE")) ? Convert.ToInt32(Environment.GetEnvironmentVariable("FILESIZE")) : 100;

            var queueClient = new QueueClient(connectionString, queueName, ReceiveMode.ReceiveAndDelete);
            try
            {
                queueClient.RegisterMessageHandler((m, token) =>
                {
                    try
                    {
                        // Serialize the Service Bus message to a JObject
                        JObject message = JObject.Parse(Encoding.UTF8.GetString(m.Body));

                        // Generating a large file and loading it into memory to simulate compute
                        Console.WriteLine("Recieved message: " + message["id"]);
                        byte[] data = new byte[fileSize * 1024 * 1024];
                        rng.NextBytes(data);
                        Stream stream = new MemoryStream(data);
                        stream.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    return Task.CompletedTask;

                }, new MessageHandlerOptions((exception) =>
                {
                    Console.WriteLine(exception.Exception.Data);
                    return Task.CompletedTask;
                })
                {
                    MaxConcurrentCalls = concurrent_size
                });
            }
            finally
            {
            }
        }

    }
}