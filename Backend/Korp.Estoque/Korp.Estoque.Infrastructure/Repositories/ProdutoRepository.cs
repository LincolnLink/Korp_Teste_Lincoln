using Korp.Estoque.Domain.Entities;
using Korp.Estoque.Domain.Interfaces;
using Korp.Estoque.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Korp.Estoque.Infrastructure.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly EstoqueDbContext _context;

        public ProdutoRepository(EstoqueDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Produto>> ObterTodosAsync()
        {
            return await _context.Produtos
                .AsNoTracking()
                .Where(x => x.Ativo)
                .ToListAsync();
        }

        public async Task<Produto?> ObterPorIdAsync(Guid id)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(x => x.Id == id && x.Ativo);
        }

        public async Task<Produto?> ObterPorCodigoAsync(string codigo)
        {
            return await _context.Produtos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Codigo == codigo && x.Ativo);
        }

        public async Task AtualizarAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirAsync(Produto produto)
        {
            produto.Ativo = false;

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Produto>> ObterPorIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.Produtos
                .Where(x => ids.Contains(x.Id) && x.Ativo)
                .ToListAsync();
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
