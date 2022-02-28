using MassTransit;
using Masstransit.RabbitMQ.Configuration.First.Infrastructure;
using Masstransit.RabbitMQ.Configuration.First.Infrastructure.Configurations;
using Masstransit.RabbitMQ.Configuration.First.Sample.Consumers;
using Masstransit.RabbitMQ.Configuration.First.Sample.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Masstransit.RabbitMQ.Configuration.First.Sample.Infrastructure
{
    public static class RabbitMQExtensions
    {
        public static void AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMQConfig = configuration.GetSection(nameof(RabbitMQConfiguration)).Get<RabbitMQConfiguration>();
            var consumerConfigs =
                configuration.GetSection(nameof(ConsumerConfigurations)).Get<ConsumerConfigurations>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<HelloMessageConsumer>();
                x.AddConsumer<OrderCancellationConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMQConfig.Host, rabbitMQConfig.Port, rabbitMQConfig.VirtualHost, h =>
                    {
                        h.Username(rabbitMQConfig.UserName);
                        h.Password(rabbitMQConfig.Password);
                    });

                    cfg.ConfigureConsumer<HelloMessageConsumer, HelloMessageContract>(
                        consumerConfigs.HelloMessageConsumer, context);

                    cfg.ConfigureConsumer<OrderCancellationConsumer, OrderPlacedContract>(
                        consumerConfigs.UnpaidOrderCancellationConsumer, context);
                });
            });

            services.RegisterDelayFirstFilters();

            services.AddMassTransitHostedService();
        }
    }
}