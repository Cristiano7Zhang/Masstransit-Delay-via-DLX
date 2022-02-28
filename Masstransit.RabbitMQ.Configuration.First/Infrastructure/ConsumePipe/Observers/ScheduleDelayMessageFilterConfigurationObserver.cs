using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.PipeConfigurators;
using Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Specifications;

namespace Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Observers
{
    public class ScheduleDelayMessageFilterConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        private readonly int _redeliveryLimit;

        public ScheduleDelayMessageFilterConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator,
            int redeliveryLimit)
            : base(receiveEndpointConfigurator)
        {
            Connect(this);
            _redeliveryLimit = redeliveryLimit;
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var specification = new ScheduleDelayMessagePipeSpecification<TMessage>(_redeliveryLimit);

            configurator.AddPipeSpecification(specification);
        }
    }
}