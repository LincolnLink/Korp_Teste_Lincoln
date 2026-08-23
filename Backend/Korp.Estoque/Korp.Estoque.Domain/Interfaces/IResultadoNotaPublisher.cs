using Korp.Estoque.Domain.Messages;

namespace Korp.Estoque.Domain.Interfaces
{
    public interface IResultadoNotaPublisher
    {
        Task PublicarAsync(ResultadoProcessamentoNotaMessage message);
    }
}
