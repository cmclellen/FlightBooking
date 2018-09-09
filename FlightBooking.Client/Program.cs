using Autofac;
using FlightBooking.Core;
using FlightBooking.Core.Extensions;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using MassTransit.RabbitMqTransport;
using MassTransit.Util;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Threading.Tasks;

namespace FlightBooking.Client
{
    class Program
    {
        private readonly RabbitMqSettings rabbitMqSettings;
        private readonly ILogger logger;

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               .WriteTo.ColoredConsole()
               .CreateLogger();

            var container = IoC.Configure();
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var program = lifetimeScope.Resolve<Program>();
                program.Run().GetAwaiter().GetResult();
            }                
        }

        public Program(RabbitMqSettings rabbitMqSettings, ILogger logger)
        {
            this.rabbitMqSettings = rabbitMqSettings;
            this.logger = logger;
        }

        static IRabbitMqHost _host;

        private async Task Run()
        {
            logger.Information("Starting...");

            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                _host = x.CreateRabbitMqHost(rabbitMqSettings);
            });

            await busControl.StartAsync();
            try
            {
                Console.Write("username: ");
                var username = Console.ReadLine();

                while (true)
                {

                    Console.Write("[add; submit]: ");
                    var text = Console.ReadLine();
                    switch(text)
                    {
                        case "add":
                            await busControl.Publish<CartItemAdded>(new
                            {
                                Username = username,
                                Timestamp = DateTimeOffset.UtcNow,
                            });
                            break;

                        case "submit":
                            Console.Write("cart id: ");
                            var cartId = Console.ReadLine();
                            await busControl.Publish<OrderSubmitted>(new
                            {
                                OrderId= new Guid(),                            
                                CartId= new Guid(cartId),
                                Username = username,
                                Timestamp = DateTimeOffset.UtcNow,
                            });
                            break;
                    }
                }
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
