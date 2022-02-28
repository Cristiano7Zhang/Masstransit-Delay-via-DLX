using System.Collections.Generic;
using GreenPipes;
using MassTransit;
using Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Filters;

namespace Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Specifications
{
    public class ScheduleDelayMessagePipeSpecification<T> :
        IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        private readonly int _redeliveryLimit;

        public ScheduleDelayMessagePipeSpecification(int redeliveryLimit)
        {
            _redeliveryLimit = redeliveryLimit;
        }

        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            var filter = new ScheduleDelayMessageFilter<T>(_redeliveryLimit);

            builder.AddFilter(filter);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_redeliveryLimit <= 0)
                yield return this.Failure("Redelivery Limit", "Redelivery limit cannot be no more than 0");
        }
    }
}