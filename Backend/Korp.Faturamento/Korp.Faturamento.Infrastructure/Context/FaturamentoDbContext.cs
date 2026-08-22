using Korp.Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Korp.Faturamento.Infrastructure.Context
{
    public class FaturamentoDbContext : DbContext
    {
        public FaturamentoDbContext(
            DbContextOptions<FaturamentoDbContext> options)
            : base(options)
        {
        }

        public DbSet<NotaFiscal> NotasFiscais { get; set; }
        public DbSet<ItemNotaFiscal> ItensNotaFiscal { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(FaturamentoDbContext).Assembly);
        }
    }
}