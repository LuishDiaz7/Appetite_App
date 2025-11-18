using Appetite_App.Models;

namespace Appetite_App.Repositories
{
    // Define la interfaz del repositorio
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByEmailAndPasswordAsync(string email, string passwordHash);
        Task<Usuario?> GetByIdAsync(int id);
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string email);
        Task<Usuario?> GetByEmailAsync(string email);
    }
}
