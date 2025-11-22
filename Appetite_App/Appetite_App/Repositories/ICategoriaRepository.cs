using System.Collections.Generic;
using System.Threading.Tasks;
using Appetite_App.Models;

namespace Appetite_App.Repositories
{
    /// <summary>
    /// Define el contrato (interfaz) para el repositorio de la entidad <see cref="Categoria"/>.
    /// Proporciona métodos para el acceso a datos y la administración de categorías.
    /// Sigue el Patrón Repositorio.
    /// </summary>
    public interface ICategoriaRepository
    {
        /// <summary>
        /// Obtiene de forma asíncrona una colección de todas las categorías disponibles.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona, conteniendo una colección de <see cref="Categoria"/>.</returns>
        Task<IEnumerable<Categoria>> GetAllAsync();

        /// <summary>
        /// Obtiene una categoría específica por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la categoría.</param>
        /// <returns>Una tarea que representa la operación asíncrona, conteniendo la <see cref="Categoria"/> si es encontrada; de lo contrario, <c>null</c>.</returns>
        Task<Categoria?> GetByIdAsync(int id);

        /// <summary>
        /// Agrega una nueva categoría a la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="categoria">El objeto <see cref="Categoria"/> a añadir.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AddAsync(Categoria categoria);

        /// <summary>
        /// Actualiza una categoría existente en la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="categoria">El objeto <see cref="Categoria"/> con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task UpdateAsync(Categoria categoria);

        /// <summary>
        /// Elimina una categoría de la base de datos por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la categoría a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// Obtiene una colección de todas las categorías, incluyendo sus <see cref="Producto"/> asociados (eager loading).
        /// Esto es útil para mostrar un catálogo completo por categoría.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona, conteniendo una colección de <see cref="Categoria"/> con sus productos.</returns>
        Task<IEnumerable<Categoria>> GetAllWithProductsAsync();

    }
}
