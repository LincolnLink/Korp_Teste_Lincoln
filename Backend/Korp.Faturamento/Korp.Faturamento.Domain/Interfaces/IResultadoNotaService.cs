using Korp.Faturamento.Domain.Messages;

namespace Korp.Faturamento.Domain.Interfaces
{
    public interface IResultadoNotaService
    {
        Task ProcessarResultadoAsync(
            ResultadoProcessamentoNotaMessage message);
    }
}
