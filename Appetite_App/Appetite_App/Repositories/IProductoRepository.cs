using Appetite_App.Models;

namespace Appetite_App.Repositories
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<IEnumerable<Producto>> GetAllWithCategoryAsync();
        Task<Producto?> GetByIdAsync(int id);
        // Métodos de administración:
        Task AddAsync(Producto producto);
        Task UpdateAsync(Producto producto);
        Task DeleteAsync(int id);
        Task<IEnumerable<Producto>> GetByCategoryIdAsync(int categoryId);

    }
}
