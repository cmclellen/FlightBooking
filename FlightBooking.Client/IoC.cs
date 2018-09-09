using Autofac;
using AutofacSerilogIntegration;
using FlightBooking.Core.Autofac;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FlightBooking.Client
{
    static class IoC
    {
        internal static IContainer Configure()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CoreModule>();
            builder.RegisterType<Program>().AsSelf();            
            var container = builder.Build();
            return container;
        }
    }
}
