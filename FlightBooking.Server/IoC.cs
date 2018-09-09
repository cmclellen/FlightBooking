using Autofac;
using FlightBooking.Core.Autofac;
using FlightBooking.Server.Data;
using Microsoft.Extensions.Configuration;

namespace FlightBooking.Server
{
    static class IoC
    {
        internal static IContainer Configure()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CoreModule>();
            builder.RegisterType<Program>().AsSelf();
            builder.RegisterType<FlightBookingDbContext>().InstancePerDependency();
            var container = builder.Build();
            return container;
        }
    }
}
