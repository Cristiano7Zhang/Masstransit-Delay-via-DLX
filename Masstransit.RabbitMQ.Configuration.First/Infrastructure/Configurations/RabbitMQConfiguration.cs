namespace Masstransit.RabbitMQ.Configuration.First.Infrastructure.Configurations
{
    public class RabbitMQConfiguration
    {
        public string Host { get; set; }

        public ushort Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }
    }
}