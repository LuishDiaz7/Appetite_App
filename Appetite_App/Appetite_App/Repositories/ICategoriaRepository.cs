using Appetite_App.Models;

namespace Appetite_App.Repositories
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(int id);

        Task AddAsync(Categoria categoria); 
        Task UpdateAsync(Categoria categoria);
        Task DeleteAsync(int id);
        Task<IEnumerable<Categoria>> GetAllWithProductsAsync();

    }
}
