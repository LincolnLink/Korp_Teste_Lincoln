using Korp.Faturamento.Domain.Messages;

namespace Korp.Faturamento.Domain.Interfaces
{
    public interface IProcessamentoNotaPublisher
    {
        Task PublicarProcessamentoNotaAsync(ProcessarNotaFiscalMessage message);
    }
}
