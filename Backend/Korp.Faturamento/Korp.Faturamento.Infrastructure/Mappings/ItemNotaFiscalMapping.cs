using Korp.Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Korp.Faturamento.Infrastructure.Mappings
{
    public class ItemNotaFiscalMapping
        : IEntityTypeConfiguration<ItemNotaFiscal>
    {
        public void Configure(
            EntityTypeBuilder<ItemNotaFiscal> builder)
        {
            builder.ToTable("ItensNotaFiscal");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NotaFiscalId)
                .IsRequired();

            builder.Property(x => x.ProdutoId)
                .IsRequired();

            builder.Property(x => x.Quantidade)
                .IsRequired();
        }
    }
}