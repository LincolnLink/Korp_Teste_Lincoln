using Korp.Estoque.Domain.Entities;

namespace Korp.Estoque.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Task AdicionarAsync(Produto produto);

        Task<IEnumerable<Produto>> ObterTodosAsync();

        Task<Produto?> ObterPorIdAsync(Guid id);

        Task<Produto?> ObterPorCodigoAsync(string codigo);

        Task AtualizarAsync(Produto produto);

        Task ExcluirAsync(Produto produto);

        Task<List<Produto>> ObterPorIdsAsync(IEnumerable<Guid> ids);

        Task SalvarAlteracoesAsync();
    }
}
