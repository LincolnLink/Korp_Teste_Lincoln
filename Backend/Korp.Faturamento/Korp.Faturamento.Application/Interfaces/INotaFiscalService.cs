using Korp.Faturamento.Application.DTOs;

namespace Korp.Faturamento.Application.Interfaces
{
    public interface INotaFiscalService
    {
        Task<NotaFiscalResponseDto> CadastrarAsync(
            CriarNotaFiscalDto request);

        Task<IEnumerable<NotaFiscalResponseDto>> ObterTodosAsync();

        Task<NotaFiscalResponseDto> ObterPorIdAsync(Guid id);

        Task ImprimirAsync(Guid id);
    }
}
