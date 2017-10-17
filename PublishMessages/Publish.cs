using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace publish_queue_bulk
{
    class Publish
    {
        public static async Task StartAsync(int message_size)
        {
            var connectionString = Environment.GetEnvironmentVariable("QUEUE_CONNECTIONSTRING");
            var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME");

            var queueClient = new QueueClient(connectionString, queueName);
            try
            {
                await Task.Run(() => Parallel.For(0, message_size, new ParallelOptions { MaxDegreeOfParallelism = 50 }, x =>
                {
                    queueClient.SendAsync(GenerateMessage()).GetAwaiter().GetResult();
                    Console.WriteLine($"Sent message {x}");
                }));

            }
            finally { }
        }

        private static Message GenerateMessage()
        {
            return new Message(Encoding.UTF8.GetBytes($"{{\"id\": \"${Guid.NewGuid().ToString()}\"}}"));
        }

    }
}