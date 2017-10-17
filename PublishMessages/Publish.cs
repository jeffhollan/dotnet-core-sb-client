using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
                List<Task> tasks = new List<Task>();
                for(int x = 0; x < message_size; x++) 
                {
                    tasks.Add(queueClient.SendAsync(GenerateMessage()));
                    Console.WriteLine($"Sent message {x}");
                }

                await Task.WhenAll(tasks.ToArray());

            }
            finally { }
        }

        private static Message GenerateMessage()
        {
            return new Message(Encoding.UTF8.GetBytes($"{{\"id\": \"${Guid.NewGuid().ToString()}\"}}"));
        }

    }
}