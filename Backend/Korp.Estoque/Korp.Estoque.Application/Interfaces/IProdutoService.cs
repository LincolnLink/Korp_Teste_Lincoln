using System;
using System.Collections.Generic;
using System.Text;

namespace Korp.Estoque.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<ProdutoResponseDto> CadastrarAsync(ProdutoRequestDto request);

        Task<IEnumerable<ProdutoResponseDto>> ObterTodosAsync();

        Task<ProdutoResponseDto?> ObterPorIdAsync(Guid id);

        Task<bool> AtualizarAsync(Guid id, ProdutoRequestDto request);

        Task<bool> ExcluirAsync(Guid id);
    }
}
