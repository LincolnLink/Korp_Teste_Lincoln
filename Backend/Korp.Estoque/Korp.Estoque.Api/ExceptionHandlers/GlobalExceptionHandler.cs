using FluentValidation;
using Korp.Estoque.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Korp.Estoque.Api.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(
                exception,
                "Erro ao processar a requisição.");

            var problemDetails = new ProblemDetails();

            switch (exception)
            {
                case ValidationException validationException:

                    problemDetails.Status =
                        StatusCodes.Status400BadRequest;

                    problemDetails.Title =
                        "Erro de validação";

                    problemDetails.Detail = string.Join(
                        " | ",
                        validationException.Errors
                            .Select(x => x.ErrorMessage));

                    break;

                case BusinessException:

                    problemDetails.Status =
                        StatusCodes.Status400BadRequest;

                    problemDetails.Title =
                        "Erro de negócio";

                    problemDetails.Detail =
                        exception.Message;

                    break;

                case NotFoundException:

                    problemDetails.Status =
                        StatusCodes.Status404NotFound;

                    problemDetails.Title =
                        "Recurso não encontrado";

                    problemDetails.Detail =
                        exception.Message;

                    break;

                default:

                    problemDetails.Status =
                        StatusCodes.Status500InternalServerError;

                    problemDetails.Title =
                        "Erro interno";

                    problemDetails.Detail =
                        "Ocorreu um erro interno ao processar a solicitação.";

                    break;
            }

            httpContext.Response.StatusCode =
                problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken);

            return true;
        }
    }
}