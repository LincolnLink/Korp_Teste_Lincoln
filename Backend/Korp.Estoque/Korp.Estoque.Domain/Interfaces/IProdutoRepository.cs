using Korp.Estoque.Domain.Entities;

namespace Korp.Estoque.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Task AdicionarAsync(Produto produto);

        Task<IEnumerable<Produto>> ObterTodosAsync();

        Task<Produto?> ObterPorIdAsync(Guid id);

        Task AtualizarAsync(Produto produto);

        Task ExcluirAsync(Produto produto);
    }
}
