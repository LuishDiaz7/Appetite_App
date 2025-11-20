using Microsoft.EntityFrameworkCore;
using Appetite_App.Data;
using Appetite_App.Models;

namespace Appetite_App.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AppetiteContext _context;

        public CategoriaRepository(AppetiteContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            return await _context.Categorias.ToListAsync();
        }

        public async Task<Categoria?> GetByIdAsync(int id)
        {
            return await _context.Categorias.FindAsync(id);
        }

        public async Task AddAsync(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
        }

        // Implementación de UpdateAsync
        public async Task UpdateAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        // Implementación de DeleteAsync
        public async Task DeleteAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Categoria>> GetAllWithProductsAsync()
        {
            return await _context.Categorias
                                 .Include(c => c.Productos)
                                 .ToListAsync();
        }

    }
}
