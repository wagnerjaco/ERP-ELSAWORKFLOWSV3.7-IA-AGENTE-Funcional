using ERP.Domain;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Marca> Marcas => Set<Marca>();
        public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
        public DbSet<ProdutoFornecedor> ProdutoFornecedores => Set<ProdutoFornecedor>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<PedidoCompra> PedidoCompras => Set<PedidoCompra>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}