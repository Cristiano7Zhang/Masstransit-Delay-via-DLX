using System;
using System.Collections.Generic;
using GreenPipes;
using Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Filters;
using MassTransit.RabbitMqTransport;

namespace Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Specifications
{
    public class ConfigureDeadLetterTopologyPipeSpecification :
        IPipeSpecification<ModelContext>
    {
        private readonly DeadLetterTopologyConfiguration _deadLetterTopologyConfiguration;

        public ConfigureDeadLetterTopologyPipeSpecification(
            DeadLetterTopologyConfiguration deadLetterTopologyConfiguration)
        {
            _deadLetterTopologyConfiguration = deadLetterTopologyConfiguration;
        }

        public void Apply(IPipeBuilder<ModelContext> builder)
        {
            var filter = new ConfigureDeadLetterTopologyFilter(_deadLetterTopologyConfiguration);
            builder.AddFilter(filter);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(_deadLetterTopologyConfiguration?.Queue))
                yield return this.Failure("Queue", "Queue cannot be null");

            if (string.IsNullOrEmpty(_deadLetterTopologyConfiguration?.DeadLetterExchange))
                yield return this.Failure("DeadLetterExchange", "DeadLetterExchange cannot be null");

            if (_deadLetterTopologyConfiguration?.MessageExpiration <= TimeSpan.Zero)
                yield return this.Failure("MessageExpiration", "MessageExpiration cannot be no more than zero");
        }
    }

    public class DeadLetterTopologyConfiguration
    {
        public DeadLetterTopologyConfiguration(string queue, string deadLetterExchange, TimeSpan messageExpiration)
        {
            Queue = queue;
            DeadLetterExchange = deadLetterExchange;
            MessageExpiration = messageExpiration;
        }

        public string Queue { get; }

        public string Exchange => Queue;

        public string DeadLetterExchange { get; }

        public TimeSpan MessageExpiration { get; }
    }
}