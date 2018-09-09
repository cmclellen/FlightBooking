using MassTransit;
using MassTransit.RabbitMqTransport;
using System;

namespace FlightBooking.Core.Extensions
{
    public static class MassTransitExtensions
    {
        public static IRabbitMqHost CreateRabbitMqHost(this IRabbitMqBusFactoryConfigurator configurator, RabbitMqSettings rabbitMqSettings) {
            return configurator.Host(new Uri($"rabbitmq://{rabbitMqSettings.Host}/"), h =>
            {
                h.Username(rabbitMqSettings.Username);
                h.Password(rabbitMqSettings.Password);
            });
        }
    }
}
