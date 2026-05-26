using ERP.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Configurations
{
    public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("Produtos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Sku).HasMaxLength(30);
            builder.Property(p => p.Ean).HasMaxLength(14);
            builder.Property(p => p.Descricao).IsRequired().HasMaxLength(120);
            builder.Property(p => p.DescricaoNFe).HasMaxLength(120);
            builder.Property(p => p.DescricaoComplementar).HasMaxLength(500);

            builder.Property(p => p.Ncm).HasMaxLength(8);
            builder.Property(p => p.CEST).HasMaxLength(7);
            builder.Property(p => p.Cfop).HasMaxLength(4);
            builder.Property(p => p.CstIcms).HasMaxLength(2);
            builder.Property(p => p.CstPis).HasMaxLength(2);

            builder.Property(p => p.CustoMedio).HasColumnType("decimal(18,4)");
            builder.Property(p => p.UltimaCompra).HasColumnType("decimal(18,4)");
            builder.Property(p => p.Markup).HasColumnType("decimal(18,4)");
            builder.Property(p => p.PrecoVenda).HasColumnType("decimal(18,4)");
            builder.Property(p => p.PrecoMinimo).HasColumnType("decimal(18,4)");
            builder.Property(p => p.AliqIcms).HasColumnType("decimal(5,2)");
            builder.Property(p => p.MvaSt).HasColumnType("decimal(5,2)");
            builder.Property(p => p.AliqPis).HasColumnType("decimal(5,4)");
            builder.Property(p => p.AliqCofins).HasColumnType("decimal(5,4)");

            builder.Property(p => p.PesoLiquido).HasColumnType("decimal(10,3)");
            builder.Property(p => p.PesoBruto).HasColumnType("decimal(10,3)");
            builder.Property(p => p.Altura).HasColumnType("decimal(10,2)");
            builder.Property(p => p.Largura).HasColumnType("decimal(10,2)");
            builder.Property(p => p.Profundidade).HasColumnType("decimal(10,2)");

            builder.Property(p => p.EstoqueMinimo).HasColumnType("decimal(18,4)");
            builder.Property(p => p.EstoqueAtual).HasColumnType("decimal(18,4)");
            builder.Property(p => p.PontoReposicao).HasColumnType("decimal(18,4)");

            builder.Property(p => p.Localizacao).HasMaxLength(50);

            builder.HasOne(p => p.Categoria).WithMany().HasForeignKey(p => p.CategoriaId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(p => p.Marca).WithMany().HasForeignKey(p => p.MarcaId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}