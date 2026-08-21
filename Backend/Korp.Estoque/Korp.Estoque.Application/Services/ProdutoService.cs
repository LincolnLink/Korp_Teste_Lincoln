using FluentValidation;
using Korp.Estoque.Application.Exceptions;
using Korp.Estoque.Application.Interfaces;
using Korp.Estoque.Domain.Entities;
using Korp.Estoque.Domain.Interfaces;

namespace Korp.Estoque.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IValidator<ProdutoRequestDto> _validator;

        public ProdutoService(
            IProdutoRepository produtoRepository,
            IValidator<ProdutoRequestDto> validator)
        {
            _produtoRepository = produtoRepository;
            _validator = validator;
        }

        public async Task<ProdutoResponseDto> CadastrarAsync(
            ProdutoRequestDto request)
        {
            await _validator.ValidateAndThrowAsync(request);

            var produtoExistente =
                await _produtoRepository.ObterPorCodigoAsync(request.Codigo);

            if (produtoExistente is not null)
            {
                throw new BusinessException(
                    $"Já existe um produto com o código {request.Codigo}.");
            }

            var produto = new Produto
            {
                Id = Guid.NewGuid(),
                Codigo = request.Codigo,
                Descricao = request.Descricao,
                Saldo = request.Saldo
            };

            await _produtoRepository.AdicionarAsync(produto);

            return MapearProduto(produto);
        }

        public async Task<IEnumerable<ProdutoResponseDto>> ObterTodosAsync()
        {
            var produtos = await _produtoRepository.ObterTodosAsync();

            return produtos.Select(MapearProduto);
        }

        public async Task<ProdutoResponseDto> ObterPorIdAsync(Guid id)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);

            if (produto is null)
            {
                throw new NotFoundException(
                    $"Produto com Id {id} não encontrado.");
            }

            return MapearProduto(produto);
        }

        public async Task AtualizarAsync(Guid id, ProdutoRequestDto request)
        {
            await _validator.ValidateAndThrowAsync(request);

            var produto = await _produtoRepository.ObterPorIdAsync(id);

            if (produto is null)
            {
                throw new NotFoundException(
                    $"Produto com Id {id} não encontrado.");
            }

            var produtoMesmoCodigo =
                await _produtoRepository.ObterPorCodigoAsync(request.Codigo);

            if (produtoMesmoCodigo is not null &&
                produtoMesmoCodigo.Id != id)
            {
                throw new BusinessException(
                    $"Já existe um produto com o código {request.Codigo}.");
            }

            produto.Codigo = request.Codigo;
            produto.Descricao = request.Descricao;
            produto.Saldo = request.Saldo;

            await _produtoRepository.AtualizarAsync(produto);
        }

        public async Task ExcluirAsync(Guid id)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);

            if (produto is null)
            {
                throw new NotFoundException(
                    $"Produto com Id {id} não encontrado.");
            }

            await _produtoRepository.ExcluirAsync(produto);
        }

        private static ProdutoResponseDto MapearProduto(Produto produto)
        {
            return new ProdutoResponseDto
            {
                Id = produto.Id,
                Codigo = produto.Codigo,
                Descricao = produto.Descricao,
                Saldo = produto.Saldo
            };
        }
    }
}
