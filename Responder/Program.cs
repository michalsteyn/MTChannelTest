using System;
using Common;
using MassTransit;

namespace Responder
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Starting Responder...");

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h => {});

                sbc.ReceiveEndpoint(host, "channel-test", ep =>
                {
                    ep.Handler<RequestMessage>(context => context.RespondAsync(
                        new ResponseMessage { Text = $"Hallo {context.Message.Text}" }
                    ));
                });
            });

            bus.Start();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            bus.Stop();
        }
    }
}
