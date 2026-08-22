using Korp.Faturamento.Application.Messages;

namespace Korp.Faturamento.Application.Interfaces
{
    public interface IRabbitMqPublisher
    {
        Task PublicarProcessamentoNotaAsync(
            ProcessarNotaFiscalMessage message);
    }
}
