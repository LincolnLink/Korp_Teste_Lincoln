namespace Korp.Estoque.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<ProdutoResponseDto> CadastrarAsync(ProdutoRequestDto request);

        Task<IEnumerable<ProdutoResponseDto>> ObterTodosAsync();

        Task<ProdutoResponseDto> ObterPorIdAsync(Guid id);

        Task AtualizarAsync(Guid id, ProdutoRequestDto request);

        Task ExcluirAsync(Guid id);
    }
}
