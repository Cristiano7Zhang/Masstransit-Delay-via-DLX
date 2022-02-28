using System;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.PipeConfigurators;
using Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Specifications;

namespace Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Observers
{
    public class InterceptDelayFirstFilterConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        private readonly Type _delayFirstFilterType;

        private readonly IServiceProvider _serviceProvider;

        public InterceptDelayFirstFilterConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator,
            Type delayFirstFilterType, IServiceProvider serviceProvider)
            : base(receiveEndpointConfigurator)
        {
            Connect(this);

            _delayFirstFilterType = delayFirstFilterType;
            _serviceProvider = serviceProvider;
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var specification =
                new InterceptDelayFirstPipeSpecification<TMessage>(_delayFirstFilterType, _serviceProvider);

            configurator.AddPipeSpecification(specification);
        }
    }
}