using Korp.Faturamento.Infrastructure.Messaging;
using Korp.Faturamento.Infrastructure.Messaging.Consumers;

namespace Korp.Faturamento.Api.Configuration
{
    public static class RabbitMqConfig
    {
        public static IServiceCollection AddRabbitMqConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.Configure<RabbitMqSettings>(
                configuration.GetSection("RabbitMq"));

            services.AddSingleton<RabbitMqConnection>();

            services.AddSingleton<RabbitMqTopology>();

            services.AddHostedService<ResultadoNotaConsumer>();

            return services;
        }
    }
}
