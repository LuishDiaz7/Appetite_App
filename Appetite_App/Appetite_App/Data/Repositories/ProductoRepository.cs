using Appetite_App.Data; 
using Appetite_App.Models;
using Appetite_App.Repositories; 
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appetite_App.Data.Repositories
{
    // Esta clase implementa la interfaz
    public class ProductoRepository : IProductoRepository
    {
        private readonly AppetiteContext _context;

        public ProductoRepository(AppetiteContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> GetAllWithCategoryAsync()
        {
            // Usamos .Include() para cargar la Categoría junto con el Producto
            return await _context.Productos
                                 .Include(p => p.Categoria)
                                 .ToListAsync();
        }

        // ... Implementaciones restantes de IProductoRepository ...

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _context.Productos.ToListAsync();
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            // Implementación de búsqueda por ID
            return await _context.Productos.FindAsync(id);
        }

        // ... Implementaciones para AddAsync, UpdateAsync, DeleteAsync ...
        public async Task AddAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
        }
    }
}
