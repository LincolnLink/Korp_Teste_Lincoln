using Korp.Estoque.Application.Exceptions;
using Korp.Estoque.Application.Interfaces;
using Korp.Estoque.Domain.Interfaces;
using Korp.Estoque.Domain.Messages;

namespace Korp.Estoque.Application.Services
{
    public class ProcessamentoEstoqueService : IProcessamentoEstoqueService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProcessamentoEstoqueService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task ProcessarNotaAsync(ProcessarNotaFiscalMessage message)
        {
            if (message.Itens is null || message.Itens.Count == 0)
            {
                throw new BusinessException(
                    "A nota fiscal não possui itens.");
            }

            var ids = message.Itens
                .Select(x => x.ProdutoId)
                .Distinct()
                .ToList();

            var produtos =
                await _produtoRepository.ObterPorIdsAsync(ids);

            foreach (var item in message.Itens)
            {
                var produto =
                    produtos.FirstOrDefault(
                        x => x.Id == item.ProdutoId);

                if (produto is null)
                {
                    throw new BusinessException(
                        $"Produto {item.ProdutoId} não encontrado.");
                }

                if (item.Quantidade <= 0)
                {
                    throw new BusinessException(
                        "A quantidade deve ser maior que zero.");
                }

                if (produto.Saldo < item.Quantidade)
                {
                    throw new BusinessException(
                        $"Saldo insuficiente para o produto {produto.Codigo}. " +
                        $"Saldo disponível: {produto.Saldo}. " +
                        $"Quantidade solicitada: {item.Quantidade}.");
                }
            }

            // Primeiro validamos TODOS.
            // Só depois alteramos os saldos.
            foreach (var item in message.Itens)
            {
                var produto =
                    produtos.First(x => x.Id == item.ProdutoId);

                produto.Saldo -= item.Quantidade;
            }

            await _produtoRepository.SalvarAlteracoesAsync();
        }
    }
}
