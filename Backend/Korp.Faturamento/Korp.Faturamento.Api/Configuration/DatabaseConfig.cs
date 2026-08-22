using Korp.Faturamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Korp.Faturamento.Api.Configuration
{
    public static class DatabaseConfig
    {
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<FaturamentoDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
    }
}
