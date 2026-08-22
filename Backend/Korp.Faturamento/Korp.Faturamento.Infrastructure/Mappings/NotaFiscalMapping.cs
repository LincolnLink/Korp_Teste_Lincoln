using Korp.Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Korp.Faturamento.Infrastructure.Mappings
{
    public class NotaFiscalMapping
        : IEntityTypeConfiguration<NotaFiscal>
    {
        public void Configure(
            EntityTypeBuilder<NotaFiscal> builder)
        {
            builder.ToTable("NotasFiscais");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Numero)
                .IsRequired();

            builder.HasIndex(x => x.Numero)
                .IsUnique();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.DataCriacao)
                .IsRequired();

            builder.HasMany(x => x.Itens)
                .WithOne(x => x.NotaFiscal)
                .HasForeignKey(x => x.NotaFiscalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}