using Appetite_App.Models;

namespace Appetite_App.Repositories
{
    public interface IOrdenRepository
    {
        Task<IEnumerable<PreOrden>> GetAllAsync();
        Task<PreOrden?> GetByIdAsync(int id);
        Task AddAsync(PreOrden orden);
        Task UpdateAsync(PreOrden orden);
    }
}
