using ERP.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Configurations
{
    public class MarcaConfiguration : IEntityTypeConfiguration<Marca>
    {
        public void Configure(EntityTypeBuilder<Marca> builder)
        {
            builder.ToTable("Marcas");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Nome).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Descricao).HasMaxLength(250);
        }
    }
}