using FluentValidation;
using Korp.Faturamento.Api.ExceptionHandlers;
using Korp.Faturamento.Application.Interfaces;
using Korp.Faturamento.Application.Services;
using Korp.Faturamento.Application.Validators;
using Korp.Faturamento.Domain.Interfaces;
using Korp.Faturamento.Infrastructure.Messaging.Publishers;
using Korp.Faturamento.Infrastructure.Repositories;

namespace Korp.Faturamento.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<INotaFiscalService, NotaFiscalService>();

            services.AddScoped<IResultadoNotaService, ResultadoNotaService>();

            services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();

            services.AddScoped<IProcessamentoNotaPublisher, RabbitMqPublisher>();

            services.AddValidatorsFromAssemblyContaining<CriarNotaFiscalDtoValidator>();

            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddProblemDetails();

            return services;
        }
    }
}
