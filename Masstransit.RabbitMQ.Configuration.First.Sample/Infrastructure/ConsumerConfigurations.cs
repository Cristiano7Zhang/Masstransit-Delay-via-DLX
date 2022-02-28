using Masstransit.RabbitMQ.Configuration.First.Infrastructure.Configurations;

namespace Masstransit.RabbitMQ.Configuration.First.Sample.Infrastructure
{
    public class ConsumerConfigurations
    {
        public ConsumerConfiguration HelloMessageConsumer { get; set; }
        public ConsumerConfiguration UnpaidOrderCancellationConsumer { get; set; }
    }
}