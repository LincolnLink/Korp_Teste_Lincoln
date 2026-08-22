using Korp.Estoque.Domain.Messages;

namespace Korp.Estoque.Domain.Interfaces
{
    public interface IProcessamentoEstoqueService
    {
        Task ProcessarNotaAsync(ProcessarNotaFiscalMessage message);
    }
}
