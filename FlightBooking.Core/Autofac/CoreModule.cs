using Autofac;
using AutofacSerilogIntegration;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FlightBooking.Core.Autofac
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx => {
                return new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
            }).As<IConfiguration>();
            builder.RegisterType<RabbitMqSettings>().AsSelf();
            builder.RegisterLogger();
        }
    }
}
