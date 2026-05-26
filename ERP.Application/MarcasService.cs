using ERP.Domain;
using ERP.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application
{
    public class MarcasService
    {
        private readonly AppDbContext context;

        public MarcasService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Marca>> GetAll()
        {
            return await context.Marcas.Where(m => m.Ativa).ToListAsync();
        }

        public async Task<Marca?> GetById(Guid id)
        {
            return await context.Marcas.Where(m => m.Id == id).SingleOrDefaultAsync();
        }

        public async Task<Marca> Create(Marca marca)
        {
            if (string.IsNullOrWhiteSpace(marca.Nome))
                throw new ArgumentException("Nome é obrigatório");
            if (marca.Id == Guid.Empty)
                marca.Id = Guid.NewGuid();

            marca.CreatedAt = DateTime.UtcNow;
            context.Marcas.Add(marca);
            await context.SaveChangesAsync();
            return marca;
        }

        public async Task<Marca?> Update(Guid id, Marca input)
        {
            var marca = await context.Marcas.Where(m => m.Id == id).SingleOrDefaultAsync();
            if (marca == null)
                return null;

            marca.Nome = input.Nome;
            marca.Descricao = input.Descricao;
            marca.Ativa = input.Ativa;

            await context.SaveChangesAsync();
            return marca;
        }

        public async Task<bool> Delete(Guid id)
        {
            var marca = await context.Marcas.Where(m => m.Id == id).SingleOrDefaultAsync();
            if (marca == null) return false;
            marca.Ativa = false;
            await context.SaveChangesAsync();
            return true;
        }
    }
}