using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Azure.ServiceBus;


namespace publish_queue_bulk
{
    class Program
    {
        private static int MESSAGES = String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PUBLISH_COUNT")) ? Convert.ToInt32(Environment.GetEnvironmentVariable("PUBLISH_COUNT")) : 100;
        private static int CONCURRENCY = String.IsNullOrEmpty(Environment.GetEnvironmentVariable("CONCURRENT_READ")) ? Convert.ToInt32(Environment.GetEnvironmentVariable("CONCURRENT_READ")) : 5;

        static int Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption("-h|--help");
            var optionMode = app.Option("-m|--mode <MODE>", "Publish or listen mode", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                var mode = optionMode.HasValue()
                    ? optionMode.Value()
                    : "consume";

                if(mode.Equals("publish", StringComparison.OrdinalIgnoreCase)) {
                    Publish.StartAsync(MESSAGES).GetAwaiter().GetResult();
                }
                else {
                    Consume.Start(CONCURRENCY);
                    Console.ReadLine();
                }
                return 0;
            });

            return app.Execute(args);
        }
    }
}
