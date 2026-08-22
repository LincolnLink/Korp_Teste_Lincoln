using Korp.Faturamento.Domain.Entities;
using Korp.Faturamento.Domain.Interfaces;
using Korp.Faturamento.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Korp.Faturamento.Infrastructure.Repositories
{
    public class NotaFiscalRepository : INotaFiscalRepository
    {
        private readonly FaturamentoDbContext _context;

        public NotaFiscalRepository(
            FaturamentoDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(
            NotaFiscal notaFiscal)
        {
            await _context.NotasFiscais
                .AddAsync(notaFiscal);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<NotaFiscal>>
            ObterTodosAsync()
        {
            return await _context.NotasFiscais
                .AsNoTracking()
                .Include(x => x.Itens)
                .OrderByDescending(x => x.Numero)
                .ToListAsync();
        }

        public async Task<NotaFiscal?> ObterPorIdAsync(
            Guid id)
        {
            return await _context.NotasFiscais
                .Include(x => x.Itens)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> ObterProximoNumeroAsync()
        {
            var ultimoNumero =
                await _context.NotasFiscais
                    .MaxAsync(x => (int?)x.Numero);

            return (ultimoNumero ?? 0) + 1;
        }

        public async Task AtualizarAsync(
            NotaFiscal notaFiscal)
        {
            _context.NotasFiscais.Update(notaFiscal);

            await _context.SaveChangesAsync();
        }
    }
}
