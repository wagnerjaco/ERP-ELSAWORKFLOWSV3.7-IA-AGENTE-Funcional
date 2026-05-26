using ERP.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.UsuarioLogin).IsRequired().HasMaxLength(50);
            builder.Property(u => u.SenhaHash).IsRequired();
            builder.Property(u => u.Nome).HasMaxLength(150);
            builder.Property(u => u.Email).HasMaxLength(100);

            builder.HasIndex(u => u.UsuarioLogin).IsUnique();
        }
    }
}