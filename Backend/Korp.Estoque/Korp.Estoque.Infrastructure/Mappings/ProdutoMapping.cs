using Korp.Estoque.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Korp.Estoque.Infrastructure.Mappings
{
    public class ProdutoMapping : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("Produtos");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Codigo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Descricao)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Saldo)
                .IsRequired();

            builder.HasIndex(x => x.Codigo)
                .IsUnique();
        }
    }
}
