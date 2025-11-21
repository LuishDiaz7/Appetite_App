using Appetite_App.Data;
using Appetite_App.Models;
using Appetite_App.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq; // Necesario para .Where()
using System.Threading.Tasks;


namespace Appetite_App.Data.Repositories
{
    // Esta clase implementa la interfaz IProductoRepository
    public class ProductoRepository : IProductoRepository
    {
        private readonly AppetiteContext _context;

        public ProductoRepository(AppetiteContext context)
        {
            _context = context;
        }

        // Carga todos los productos sin incluir la categoría (útil para administración)
        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _context.Productos.ToListAsync();
        }

        // Carga todos los productos con su respectiva categoría incluida
        public async Task<IEnumerable<Producto>> GetAllWithCategoryAsync()
        {
            // Usamos .Include() para cargar la Categoría junto con el Producto
            return await _context.Productos
                .Include(p => p.Categoria)
                .ToListAsync();
        }

        // Obtiene un producto específico por su ID
        public async Task<Producto?> GetByIdAsync(int id)
        {
            // Implementación de búsqueda por ID
            return await _context.Productos.FindAsync(id);
        }

        // Obtiene productos filtrados por una categoría específica
        public async Task<IEnumerable<Producto>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Productos
                      .Where(p => p.IdCategoria == categoryId)
                      .Include(p => p.Categoria) // Opcional, dependiendo si la vista Catalogo lo necesita
                                       .ToListAsync();
        }

        // Agrega un nuevo producto a la base de datos
        public async Task AddAsync(Producto producto) 
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            
        }

        // Actualiza un producto existente en la base de datos
        public async Task UpdateAsync(Producto producto)
        {
            // Marcar el estado como modificado
            _context.Entry(producto).State = EntityState.Modified;

            // Persistir los cambios
            await _context.SaveChangesAsync();
        }

        // Elimina un producto por su ID
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
