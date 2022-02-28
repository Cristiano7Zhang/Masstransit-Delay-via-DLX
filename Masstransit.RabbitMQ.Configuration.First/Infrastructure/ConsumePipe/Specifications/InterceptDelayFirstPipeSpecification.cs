using System;
using System.Collections.Generic;
using GreenPipes;
using GreenPipes.Internals.Extensions;
using MassTransit;
using Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Filters;

namespace Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Specifications
{
    public class InterceptDelayFirstPipeSpecification<T> :
        IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        private readonly Type _delayFirstFilterType;

        private readonly IServiceProvider _serviceProvider;

        public InterceptDelayFirstPipeSpecification(Type delayFirstFilterType, IServiceProvider serviceProvider)
        {
            _delayFirstFilterType = delayFirstFilterType;
            _serviceProvider = serviceProvider;
        }

        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            var filter = new InterceptDelayFirstFilter<T>(_delayFirstFilterType, _serviceProvider);

            builder.AddFilter(filter);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_delayFirstFilterType != null && _delayFirstFilterType.HasInterface(typeof(IDelayFirstFilter<T>)))
                yield return this.Failure("Delay First Filter Type",
                    $"Delay First Filter doesn't implement {typeof(IDelayFirstFilter<T>)}");
        }
    }
}