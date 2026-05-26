using ERP.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Configurations
{
    public class FornecedorConfiguration : IEntityTypeConfiguration<Fornecedor>
    {
        public void Configure(EntityTypeBuilder<Fornecedor> builder)
        {
            builder.ToTable("Fornecedores");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Nome).IsRequired().HasMaxLength(150);
            builder.Property(f => f.NomeFantasia).HasMaxLength(150);
            builder.Property(f => f.CpfCnpj).HasMaxLength(18);
            builder.Property(f => f.RgIe).HasMaxLength(20);
            builder.Property(f => f.Email).HasMaxLength(100);
            builder.Property(f => f.Telefone).HasMaxLength(20);
            builder.Property(f => f.Celular).HasMaxLength(20);
            builder.Property(f => f.Cep).HasMaxLength(10);
            builder.Property(f => f.Endereco).HasMaxLength(200);
            builder.Property(f => f.Numero).HasMaxLength(20);
            builder.Property(f => f.Complemento).HasMaxLength(100);
            builder.Property(f => f.Bairro).HasMaxLength(100);
            builder.Property(f => f.Cidade).HasMaxLength(100);
            builder.Property(f => f.Uf).HasMaxLength(2);
            builder.Property(f => f.Observacoes).HasMaxLength(1000);
        }
    }

    public class ProdutoFornecedorConfiguration : IEntityTypeConfiguration<ProdutoFornecedor>
    {
        public void Configure(EntityTypeBuilder<ProdutoFornecedor> builder)
        {
            builder.ToTable("ProdutoFornecedores");

            builder.HasKey(pf => new { pf.ProdutoId, pf.FornecedorId });

            builder.Property(pf => pf.ProdutoId).HasColumnName("ProdutoId");
            builder.Property(pf => pf.FornecedorId).HasColumnName("FornecedorId");
            builder.Property(pf => pf.CodigoFornecedor).HasMaxLength(50);
            builder.Property(pf => pf.PrecoFornecedor).HasColumnType("decimal(18,4)");

            builder.HasOne(pf => pf.Produto)
                .WithMany(p => p.ProdutoFornecedores)
                .HasForeignKey(pf => pf.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(pf => pf.Fornecedor)
                .WithMany()
                .HasForeignKey(pf => pf.FornecedorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}