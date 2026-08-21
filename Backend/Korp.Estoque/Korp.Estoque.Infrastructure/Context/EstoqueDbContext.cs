using Korp.Estoque.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Korp.Estoque.Infrastructure.Context
{
    public class EstoqueDbContext : DbContext
    {
        public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(EstoqueDbContext).Assembly);
        }
    }
}
