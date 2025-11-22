using Microsoft.EntityFrameworkCore;
using Appetite_App.Data;
using Appetite_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Appetite_App.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio para la entidad <see cref="Usuario"/>.
    /// Utiliza <see cref="AppetiteContext"/> (que debe heredar de IdentityDbContext) y Entity Framework Core para la persistencia.
    /// Sigue el Patrón Repositorio y aísla el código de EF Core.
    /// </summary>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppetiteContext _context;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de usuarios.
        /// </summary>
        /// <param name="context">El contexto de la base de datos de la aplicación, inyectado vía DI.</param>
        public UsuarioRepository(AppetiteContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene un usuario buscando por su email y su hash de contraseña almacenado.
        /// (Implementación de <see cref="IUsuarioRepository.GetByEmailAndPasswordAsync"/>).
        /// </summary>
        /// <param name="email">El correo electrónico del usuario.</param>
        /// <param name="passwordHash">El valor hasheado de la contraseña del usuario.</param>
        /// <returns>Una tarea que devuelve el <see cref="Usuario"/> si coincide la combinación; de lo contrario, <c>null</c>.</returns>
        public async Task<Usuario?> GetByEmailAndPasswordAsync(string email, string passwordHash)
        {
            // Nota de Implementación: La validación real de la contraseña (usando VerifyHashedPassword)
            // debería residir en un servicio de autenticación para desacoplar el repositorio.
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == passwordHash);
        }

        /// <summary>
        /// Obtiene un usuario específico por su identificador único de forma asíncrona.
        /// (Implementación de <see cref="IUsuarioRepository.GetByIdAsync"/>).
        /// </summary>
        /// <param name="id">El identificador del usuario.</param>
        /// <returns>Una tarea que devuelve el <see cref="Usuario"/> si es encontrado; de lo contrario, <c>null</c>.</returns>
        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        /// <summary>
        /// Obtiene una colección de todos los usuarios registrados en el sistema de forma asíncrona.
        /// (Implementación de <see cref="IUsuarioRepository.GetAllAsync"/>).
        /// </summary>
        /// <returns>Una tarea que devuelve una colección de <see cref="Usuario"/>.</returns>
        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        /// <summary>
        /// Agrega un nuevo usuario a la base de datos y persiste los cambios de forma asíncrona.
        /// (Implementación de <see cref="IUsuarioRepository.AddAsync"/>).
        /// </summary>
        /// <param name="usuario">El objeto <see cref="Usuario"/> a añadir.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza un usuario existente en la base de datos y persiste los cambios de forma asíncrona.
        /// (Implementación de <see cref="IUsuarioRepository.UpdateAsync"/>).
        /// </summary>
        /// <param name="usuario">El objeto <see cref="Usuario"/> con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina un usuario de la base de datos por su identificador único de forma asíncrona.
        /// (Implementación de <see cref="IUsuarioRepository.DeleteAsync"/>).
        /// </summary>
        /// <param name="id">El identificador del usuario a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task DeleteAsync(int id)
        {
            Usuario? usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Verifica si un usuario con un correo electrónico específico ya existe en el sistema de forma asíncrona.
        /// (Implementación de <see cref="IUsuarioRepository.ExistsAsync"/>).
        /// </summary>
        /// <param name="email">El correo electrónico a verificar.</param>
        /// <returns>Una tarea que devuelve <c>true</c> si el usuario existe; de lo contrario, <c>false</c>.</returns>
        public async Task<bool> ExistsAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Obtiene un usuario por su correo electrónico de forma asíncrona.
        /// (Implementación de <see cref="IUsuarioRepository.GetByEmailAsync"/>).
        /// </summary>
        /// <param name="email">El correo electrónico del usuario.</param>
        /// <returns>Una tarea que devuelve el <see cref="Usuario"/> si es encontrado; de lo contrario, <c>null</c>.</returns>
        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
