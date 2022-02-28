using System.Collections.Generic;
using System.Threading.Tasks;
using GreenPipes;
using Masstransit.RabbitMQ.Configuration.First.Commons.Constants;
using Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Specifications;
using MassTransit.RabbitMqTransport;
using RabbitMQ.Client;

namespace Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Filters
{
    public class ConfigureDeadLetterTopologyFilter :
        IFilter<ModelContext>
    {
        private readonly DeadLetterTopologyConfiguration _deadLetterTopologyConfiguration;

        public ConfigureDeadLetterTopologyFilter(DeadLetterTopologyConfiguration deadLetterTopologyConfiguration)
        {
            _deadLetterTopologyConfiguration = deadLetterTopologyConfiguration;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope(nameof(ConfigureDeadLetterTopologyFilter));
            scope.Add("type", "dead letter topology");
        }

        public async Task Send(ModelContext context, IPipe<ModelContext> next)
        {
            await context.ExchangeDeclare(_deadLetterTopologyConfiguration.Exchange, ExchangeType.Direct, true, false,
                null);
            await context.QueueDeclare(_deadLetterTopologyConfiguration.Queue, true, false, false,
                new Dictionary<string, object>
                {
                    {
                        QueueArguments.MessageExpiration,
                        (int)_deadLetterTopologyConfiguration.MessageExpiration.TotalMilliseconds
                    },
                    { QueueArguments.DeadLetterExchange, _deadLetterTopologyConfiguration.DeadLetterExchange }
                });
            await context.QueueBind(_deadLetterTopologyConfiguration.Exchange, _deadLetterTopologyConfiguration.Queue,
                string.Empty, null);

            await next.Send(context);
        }
    }
}