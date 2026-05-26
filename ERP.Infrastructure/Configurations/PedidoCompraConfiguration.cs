using ERP.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Configurations
{
    public class PedidoCompraConfiguration : IEntityTypeConfiguration<PedidoCompra>
    {
        public void Configure(EntityTypeBuilder<PedidoCompra> builder)
        {
            builder.ToTable("PedidoCompras");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.NumeroPedido).IsRequired().HasMaxLength(20);
            builder.Property(p => p.Descricao).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Observacao).HasMaxLength(500);
            builder.Property(p => p.EmailAprovacao).HasMaxLength(100);

            builder.Property(p => p.Quantidade).HasColumnType("decimal(18,4)");
            builder.Property(p => p.CustoMedio).HasColumnType("decimal(18,4)");
            builder.Property(p => p.ValorTotal).HasColumnType("decimal(18,4)");

            builder.HasOne(p => p.Fornecedor).WithMany().HasForeignKey(p => p.FornecedorId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Categoria).WithMany().HasForeignKey(p => p.CategoriaId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(p => p.Marca).WithMany().HasForeignKey(p => p.MarcaId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(p => p.Produto).WithMany().HasForeignKey(p => p.ProdutoId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}