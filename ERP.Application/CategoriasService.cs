using ERP.Domain;
using ERP.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application
{
    public class CategoriasService
    {
        private readonly AppDbContext context;

        public CategoriasService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Categoria>> GetAll()
        {
            return await context.Categorias.Where(c => c.Ativa).ToListAsync();
        }

        public async Task<Categoria?> GetById(Guid id)
        {
            return await context.Categorias.Where(c => c.Id == id).SingleOrDefaultAsync();
        }

        public async Task<Categoria> Create(Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nome))
                throw new ArgumentException("Nome é obrigatório");
            if (categoria.Id == Guid.Empty)
                categoria.Id = Guid.NewGuid();

            categoria.CreatedAt = DateTime.UtcNow;
            context.Categorias.Add(categoria);
            await context.SaveChangesAsync();
            return categoria;
        }

        public async Task<Categoria?> Update(Guid id, Categoria input)
        {
            var categoria = await context.Categorias.Where(c => c.Id == id).SingleOrDefaultAsync();
            if (categoria == null)
                return null;

            categoria.Nome = input.Nome;
            categoria.Descricao = input.Descricao;
            categoria.Tipo = input.Tipo;
            categoria.Ativa = input.Ativa;

            await context.SaveChangesAsync();
            return categoria;
        }

        public async Task<bool> Delete(Guid id)
        {
            var categoria = await context.Categorias.Where(c => c.Id == id).SingleOrDefaultAsync();
            if (categoria == null) return false;
            categoria.Ativa = false;
            await context.SaveChangesAsync();
            return true;
        }
    }
}