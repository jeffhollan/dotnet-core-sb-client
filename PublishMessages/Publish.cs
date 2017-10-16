using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace publish_queue_bulk
{
    class Publish
    {
        public static void Start(int message_size)
        {
            var connectionString = Environment.GetEnvironmentVariable("QUEUE_CONNECTIONSTRING");
            var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME");

            var queueClient = new QueueClient(connectionString, queueName);
            try
            {
                Parallel.For(0, message_size, x =>
                {
                    queueClient.SendAsync(GenerateMessage());
                    Console.WriteLine($"Sent message {x}");
                });
            }
            finally
            {
            }
            Console.ReadLine();
        }

        private static Message GenerateMessage()
        {
            return new Message(Encoding.UTF8.GetBytes($"{{id={Guid.NewGuid().ToString()}}}"));
        }

    }
}