using Microsoft.EntityFrameworkCore;
using Appetite_App.Data;
using Appetite_App.Models;

namespace Appetite_App.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppetiteContext _context;

        public UsuarioRepository(AppetiteContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByEmailAndPasswordAsync(string email, string passwordHash)
        {
            // Nota: En un entorno de producción, nunca se pasaría la contraseña sin hashear.
            // Aquí, para el prototipo, hasheamos lo que el usuario ingresa para compararlo.
            // La lógica de hasheo real iría en la capa de Servicio/Lógica de Negocio (AuthService).
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == passwordHash);
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
