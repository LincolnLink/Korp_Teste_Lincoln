using Korp.Estoque.Application.Interfaces;
using Korp.Estoque.Application.Services;
using Korp.Estoque.Domain.Interfaces;
using Korp.Estoque.Infrastructure.Repositories;

namespace Korp.Estoque.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddDependencyInjection(
        this IServiceCollection services)
        {
            // Application
            services.AddScoped<IProdutoService, ProdutoService>();

            // Infrastructure
            services.AddScoped<IProdutoRepository, ProdutoRepository>();

            return services;
        }
    }
}
