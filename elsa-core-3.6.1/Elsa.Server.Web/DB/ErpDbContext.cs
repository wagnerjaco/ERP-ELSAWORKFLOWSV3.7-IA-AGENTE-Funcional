using Microsoft.EntityFrameworkCore;
using Elsa.Server.Web.DB.Class;
public class ErpDbContext : DbContext
{
    public ErpDbContext(DbContextOptions<ErpDbContext> options)
        : base(options)
    {
    }

    // Tabelas do ERP
    public DbSet<Produto> Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("Produtos");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Descricao)
                .HasMaxLength(200);

            entity.Property(x => x.EstoqueAtual);

            entity.Property(x => x.EstoqueMinimo);

            entity.Property(x => x.PontoReposicao); 
        });
    }
}

