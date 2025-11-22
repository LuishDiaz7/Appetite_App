using System.Collections.Generic;
using System.Threading.Tasks;
using Appetite_App.Models;

namespace Appetite_App.Repositories
{
    /// <summary>
    /// Define el contrato (interfaz) para el repositorio de la entidad <see cref="Usuario"/>.
    /// Proporciona métodos específicos para la gestión de usuarios, incluyendo autenticación básica y operaciones CRUD.
    /// </summary>
    public interface IUsuarioRepository
    {
        /// <summary>
        /// Obtiene un usuario buscando por su email y su hash de contraseña.
        /// Este método es útil para la autenticación manual o validación fuera del flujo estándar de Identity.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario.</param>
        /// <param name="passwordHash">El hash de la contraseña del usuario.</param>
        /// <returns>Una tarea que devuelve el <see cref="Usuario"/> si coincide la combinación; de lo contrario, <c>null</c>.</returns>
        Task<Usuario?> GetByEmailAndPasswordAsync(string email, string passwordHash);

        /// <summary>
        /// Obtiene un usuario específico por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del usuario.</param>
        /// <returns>Una tarea que devuelve el <see cref="Usuario"/> si es encontrado; de lo contrario, <c>null</c>.</returns>
        Task<Usuario?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene una colección de todos los usuarios registrados en el sistema de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que devuelve una colección de <see cref="Usuario"/>.</returns>
        Task<IEnumerable<Usuario>> GetAllAsync();

        /// <summary>
        /// Agrega un nuevo usuario a la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="usuario">El objeto <see cref="Usuario"/> a añadir.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AddAsync(Usuario usuario);

        /// <summary>
        /// Actualiza un usuario existente en la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="usuario">El objeto <see cref="Usuario"/> con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task UpdateAsync(Usuario usuario);

        /// <summary>
        /// Elimina un usuario de la base de datos por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del usuario a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// Verifica si un usuario con un correo electrónico específico ya existe en el sistema.
        /// </summary>
        /// <param name="email">El correo electrónico a verificar.</param>
        /// <returns>Una tarea que devuelve <c>true</c> si el usuario existe; de lo contrario, <c>false</c>.</returns>
        Task<bool> ExistsAsync(string email);

        /// <summary>
        /// Obtiene un usuario por su correo electrónico.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario.</param>
        /// <returns>Una tarea que devuelve el <see cref="Usuario"/> si es encontrado; de lo contrario, <c>null</c>.</returns>
        Task<Usuario?> GetByEmailAsync(string email);
    }
}
