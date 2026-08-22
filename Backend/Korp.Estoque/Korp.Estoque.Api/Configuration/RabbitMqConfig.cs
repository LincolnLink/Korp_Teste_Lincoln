using Korp.Estoque.Infrastructure.Messaging;

namespace Korp.Estoque.Api.Configuration
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

            return services;
        }
    }
}
