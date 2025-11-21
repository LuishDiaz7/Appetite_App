using System.Collections.Generic;
using System.Threading.Tasks;
using Appetite_App.Models;

namespace Appetite_App.Repositories
{
    public interface IProductoRepository
    {
        // Métodos de lectura y obtención
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<IEnumerable<Producto>> GetAllWithCategoryAsync();

        // Obtener un producto por ID (SOLO GetByIdAsync)
        Task<Producto?> GetByIdAsync(int id);

        // Obtener productos filtrados por una categoría específica
        Task<IEnumerable<Producto>> GetByCategoryIdAsync(int categoryId);

        // Métodos de administración (CRUD)
        Task AddAsync(Producto producto);
        Task UpdateAsync(Producto producto);
        Task DeleteAsync(int id);
    }
}
