using Korp.Estoque.Application.Interfaces;
using Korp.Estoque.Domain.Entities;
using Korp.Estoque.Domain.Interfaces;

namespace Korp.Estoque.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<ProdutoResponseDto> CadastrarAsync(
            ProdutoRequestDto request)
        {
            var produto = new Produto
            {
                Id = Guid.NewGuid(),
                Codigo = request.Codigo,
                Descricao = request.Descricao,
                Saldo = request.Saldo
            };

            await _produtoRepository.AdicionarAsync(produto);

            return new ProdutoResponseDto
            {
                Id = produto.Id,
                Codigo = produto.Codigo,
                Descricao = produto.Descricao,
                Saldo = produto.Saldo
            };
        }

        public async Task<IEnumerable<ProdutoResponseDto>> ObterTodosAsync()
        {
            var produtos = await _produtoRepository.ObterTodosAsync();

            return produtos.Select(produto => new ProdutoResponseDto
            {
                Id = produto.Id,
                Codigo = produto.Codigo,
                Descricao = produto.Descricao,
                Saldo = produto.Saldo
            });
        }

        public async Task<ProdutoResponseDto?> ObterPorIdAsync(Guid id)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);

            if (produto is null)
                return null;

            return new ProdutoResponseDto
            {
                Id = produto.Id,
                Codigo = produto.Codigo,
                Descricao = produto.Descricao,
                Saldo = produto.Saldo
            };
        }

        public async Task<bool> AtualizarAsync(
            Guid id,
            ProdutoRequestDto request)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);

            if (produto is null)
                return false;

            produto.Codigo = request.Codigo;
            produto.Descricao = request.Descricao;
            produto.Saldo = request.Saldo;

            await _produtoRepository.AtualizarAsync(produto);

            return true;
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);

            if (produto is null)
                return false;

            await _produtoRepository.ExcluirAsync(produto);

            return true;
        }
    }
}
