using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace publish_queue_bulk
{
    class Consume
    {
        public static void Start(int batch_size)
        {
            var connectionString = Environment.GetEnvironmentVariable("QUEUE_CONNECTIONSTRING");
            var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME");

            var queueClient = new QueueClient(connectionString, queueName, ReceiveMode.ReceiveAndDelete);
            try
            {
                queueClient.RegisterMessageHandler((message, token) =>
                {
                    Console.WriteLine(Encoding.UTF8.GetString(message.Body));
                    return Task.CompletedTask;
                }, new MessageHandlerOptions((exception) =>
                {
                    Console.WriteLine(exception.Exception.Data);
                    return Task.CompletedTask;
                })
                {
                    AutoComplete = true,
                    MaxConcurrentCalls = 1
                });
            }
            finally
            {
            }
        }

    }
}