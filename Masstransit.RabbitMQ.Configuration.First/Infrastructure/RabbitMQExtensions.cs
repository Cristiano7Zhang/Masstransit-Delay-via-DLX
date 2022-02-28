using System;
using System.Reflection;
using GreenPipes;
using MassTransit;
using Masstransit.RabbitMQ.Configuration.First.Commons.Constants;
using Masstransit.RabbitMQ.Configuration.First.Commons.Helpers;
using Masstransit.RabbitMQ.Configuration.First.Infrastructure.Configurations;
using Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe;
using Masstransit.RabbitMQ.Configuration.First.Infrastructure.ConsumePipe.Specifications;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Masstransit.RabbitMQ.Configuration.First.Infrastructure
{
    public static class RabbitMQExtensions
    {
        public static void ConfigureConsumer<TConsumer, TMessageEntity>(
            this IRabbitMqBusFactoryConfigurator busFactoryConfigurator,
            ConsumerConfiguration consumerConfig, IBusRegistrationContext context)
            where TConsumer : class, IConsumer<TMessageEntity>
            where TMessageEntity : class
        {
            if (consumerConfig.AdvancedSettings.DelayFirst.DelaySeconds > 0)
                busFactoryConfigurator.ConfigureInterceptorToDelay<TMessageEntity>(consumerConfig, context);

            busFactoryConfigurator.ReceiveEndpoint(consumerConfig.BaseSetting.Queue, ep =>
            {
                if (consumerConfig.BaseSetting.PrefetchCount >= 1)
                    ep.PrefetchCount = (ushort)consumerConfig.BaseSetting.PrefetchCount;

                ep.ConfigureConsumeTopology = false;

                // Configure binding exchange
                if (consumerConfig.AdvancedSettings.DelayFirst.DelaySeconds == 0)
                {
                    var bindExchange = consumerConfig.BaseSetting.BindExchange;
                    ep.BindExchange(bindExchange);
                }

                // Configure redelivery if enabled
                var redeliveryPolicy = consumerConfig.AdvancedSettings.Redelivery;
                if (redeliveryPolicy.Limit >= 1)
                {
                    ep.ConfigureModel(cfg =>
                    {
                        cfg.AddPipeSpecification(
                            new ConfigureDeadLetterTopologyPipeSpecification(consumerConfig
                                .ScheduleDelayMessageTopology));
                    });

                    ep.UseScheduleDelayMessageFilter(redeliveryPolicy.Limit);
                }

                // Configure retry if enabled
                var retryPolicy = consumerConfig.AdvancedSettings.Retry;
                if (retryPolicy.Limit >= 1)
                    ep.UseMessageRetry(r => r.Interval(retryPolicy.Limit, retryPolicy.DelaySeconds));

                ep.ConfigureConsumer<TConsumer>(context);
            });
        }

        public static void RegisterDelayFirstFilters(this IServiceCollection services)
        {
            var filterTypes =
                ReflectionHelper.GetClassesImplementingInterface(Assembly.GetEntryAssembly(),
                    typeof(IDelayFirstFilter<>));

            foreach (var filterType in filterTypes) services.AddTransient(filterType);
        }

        private static void ConfigureInterceptorToDelay<TMessageEntity>(
            this IRabbitMqBusFactoryConfigurator busFactoryConfigurator,
            ConsumerConfiguration consumerConfig, IBusRegistrationContext context) where TMessageEntity : class
        {
            var interceptorQueue = string.Format(QueueFormats.InterceptorQueue,
                consumerConfig.BaseSetting.BindExchange.Name, consumerConfig.BaseSetting.Queue);

            // Config receive endpoint for interceptor consumer
            busFactoryConfigurator.ReceiveEndpoint(interceptorQueue, ep =>
            {
                if (consumerConfig.BaseSetting.PrefetchCount >= 1)
                    ep.PrefetchCount = (ushort)consumerConfig.BaseSetting.PrefetchCount;

                ep.ConfigureConsumeTopology = false;

                // Configure binding exchange
                var bindExchange = consumerConfig.BaseSetting.BindExchange;
                ep.BindExchange(bindExchange);

                var delayFirstTopology = consumerConfig.DelayFirstTopology;
                ep.ConfigureModel(cfg =>
                {
                    cfg.AddPipeSpecification(new ConfigureDeadLetterTopologyPipeSpecification(delayFirstTopology));
                });

                ep.Handler<TMessageEntity>(async ctx =>
                {
                    // Choose SendEnpoint instead of PublishEndpoint here, since customizing destination exchange is not supported for PublishEndpoint
                    // See more details by https://masstransit-project.com/advanced/topology/rabbitmq.html#exchanges
                    var sendEndpoint =
                        await ctx.GetSendEndpoint(
                            new Uri($"exchange:{delayFirstTopology.Exchange}?type={ExchangeType.Direct}"));
                    await sendEndpoint.Send(ctx.Message);
                });

                var delayFirstFilter = consumerConfig.AdvancedSettings.DelayFirst.Filter;
                if (!string.IsNullOrEmpty(delayFirstFilter)) ep.UseInterceptDelayFirstFilter(delayFirstFilter, context);
            });
        }

        private static void BindExchange(this IRabbitMqReceiveEndpointConfigurator ep,
            ExchangeConfiguration bindExchange)
        {
            if (bindExchange != null)
                ep.Bind(bindExchange.Name, exchangeCfg =>
                {
                    exchangeCfg.ExchangeType = bindExchange.Type;
                    foreach (var argument in bindExchange.Arguments)
                        exchangeCfg.SetExchangeArgument(argument.Key, argument.Value);
                });
        }
    }
}