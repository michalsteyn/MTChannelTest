using System;
using System.Threading.Tasks;
using Common;
using MassTransit;

namespace Requester
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Starting Requester...");
            Console.WriteLine("Press ctrl-C to exit...");
            while (true)
            {
                SimulateProcessRestart().GetAwaiter().GetResult(); 
            }
        }

        /// <summary>
        /// This Simulates a new Short Lived Process
        /// It is clear from RabbitMQ Management that each instance creates a new unique queue.
        /// This leads to the Responder Process creating and caching a new Channel for each "process" so to speak.
        /// </summary>
        /// <returns></returns>
        static async Task SimulateProcessRestart()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("rabbitmq://localhost"), h => {});
            });
            await bus.StartAsync();

            //Is there a way to create a RequestClient and bind it to a named response queue?
            var client = bus.CreateRequestClient<RequestMessage>(new Uri("rabbitmq://localhost/channel-test"));
            var response = await client.GetResponse<ResponseMessage>(new RequestMessage {Text = "World"});

            Console.WriteLine($"Received Response: {response.Message.Text}");
            await bus.StopAsync();
        }
    }
}
