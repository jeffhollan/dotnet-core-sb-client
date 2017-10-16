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
        public static int MESSAGES = 50;

        static int Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption("-h|--help");
            var optionMode = app.Option("-m|--mode <MODE>", "Publish or listen mode", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                var mode = optionMode.HasValue()
                    ? optionMode.Value()
                    : "listen";

                if(mode.Equals("publish", StringComparison.OrdinalIgnoreCase)) {
                    Publish.Start(MESSAGES);
                }
                else {
                    
                }
                return 0;
            });

            return app.Execute(args);
        }
    }
}
