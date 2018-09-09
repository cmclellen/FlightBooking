using Microsoft.Extensions.Configuration;

namespace FlightBooking.Core
{
    public class RabbitMqSettings
    {
        private readonly IConfiguration configuration;

        public RabbitMqSettings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string Host => configuration["rabbit-mq:host"];
        public string Username => configuration["rabbit-mq:username"];
        public string Password => configuration["rabbit-mq:password"];

    }
}
