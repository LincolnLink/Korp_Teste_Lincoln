using Korp.Faturamento.Domain.Entities;

namespace Korp.Faturamento.Domain.Interfaces
{
    public interface INotaFiscalRepository
    {
        Task AdicionarAsync(NotaFiscal notaFiscal);

        Task<IEnumerable<NotaFiscal>> ObterTodosAsync();

        Task<NotaFiscal?> ObterPorIdAsync(Guid id);

        Task<int> ObterProximoNumeroAsync();

        Task AtualizarAsync(NotaFiscal notaFiscal);
    }
}
