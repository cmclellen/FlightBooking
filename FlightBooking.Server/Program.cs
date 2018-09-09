using Autofac;
using FlightBooking.Core;
using FlightBooking.Core.Logging;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Util;
using Serilog;
using Serilog.Core;
using System;
using System.Threading.Tasks;
using FlightBooking.Core.Extensions;
using GreenPipes;
using MassTransit.EntityFrameworkCoreIntegration.Saga;
using FlightBooking.Server.Data;
using MassTransit.Saga;
using Quartz;
using Quartz.Impl;
using MassTransit.QuartzIntegration;

namespace FlightBooking.Server
{
    class Program
    {
        static IContainer container;


        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               .WriteTo.ColoredConsole()
               .CreateLogger();

            container = IoC.Configure();
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var program = lifetimeScope.Resolve<Program>();
                TaskUtil.Await(() => program.Run());
            }
        }

        public Program(RabbitMqSettings rabbitMqSettings, FlightBookingDbContext flightBookingDbContext, ILogger logger)
        {
            this.rabbitMqSettings = rabbitMqSettings;
            this.flightBookingDbContext = flightBookingDbContext;
            this.logger = logger;
        }

        static IRabbitMqHost _host;
        private readonly RabbitMqSettings rabbitMqSettings;
        private readonly FlightBookingDbContext flightBookingDbContext;
        private readonly ILogger logger;
        IScheduler _scheduler;

        async Task Run() {

            logger.Information("Updating database...");
            await flightBookingDbContext.Database.EnsureCreatedAsync();
            logger.Information("done");
            _scheduler = CreateScheduler();

            var _machine = new ShoppingCartStateMachine();

            var _repository = new Lazy<ISagaRepository<ShoppingCart>>(() => new EntityFrameworkSagaRepository<ShoppingCart>(() => container.Resolve<FlightBookingDbContext>()));


            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                _host = x.CreateRabbitMqHost(rabbitMqSettings);

                x.ReceiveEndpoint(_host, "shopping_cart_state", e =>
                {
                    e.PrefetchCount = 8;
                    e.StateMachineSaga(_machine, _repository.Value);
                });

                x.ReceiveEndpoint(_host, "scheduler", e =>
                {
                    x.UseMessageScheduler(e.InputAddress);

                    e.PrefetchCount = 1;

                    e.Consumer(() => new ScheduleMessageConsumer(_scheduler));
                    e.Consumer(() => new CancelScheduledMessageConsumer(_scheduler));
                });
            });

            await busControl.StartAsync();
            try
            {

                _scheduler.JobFactory = new MassTransitJobFactory(busControl);
                await _scheduler.Start();

                while (true)
                {
                    Console.WriteLine("Type 'exit' to quit");
                    var text = Console.ReadLine();
                    if(text == "exit")
                    {
                        break;
                    }
                }    
            }
            finally
            {
                await _scheduler.Shutdown();
                await busControl.StopAsync();
            }
        }

        static IScheduler CreateScheduler()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = MassTransit.Util.TaskUtil.Await<IScheduler>(() => schedulerFactory.GetScheduler()); ;

            return scheduler;
        }
    }
}
