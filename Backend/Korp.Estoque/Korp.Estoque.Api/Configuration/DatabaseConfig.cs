using Korp.Estoque.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Korp.Estoque.Api.Configuration
{
    public static class DatabaseConfig
    {
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<EstoqueDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
    }
}
