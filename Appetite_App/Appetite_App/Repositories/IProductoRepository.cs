using System.Collections.Generic;
using System.Threading.Tasks;
using Appetite_App.Models;

namespace Appetite_App.Repositories
{
    /// <summary>
    /// Define el contrato (interfaz) para el repositorio de la entidad <see cref="Producto"/>.
    /// Sigue el Patrón Repositorio, desacoplando la lógica de negocio del acceso directo a la base de datos.
    /// </summary>
    public interface IProductoRepository
    {
        // Métodos de lectura y obtención

        /// <summary>
        /// Obtiene una colección de todos los productos disponibles.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona, conteniendo una colección de <see cref="Producto"/>.</returns>
        Task<IEnumerable<Producto>> GetAllAsync();

        /// <summary>
        /// Obtiene una colección de todos los productos, incluyendo la información de su <see cref="Categoria"/> asociada (eager loading).
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona, conteniendo una colección de <see cref="Producto"/>.</returns>
        Task<IEnumerable<Producto>> GetAllWithCategoryAsync();

        /// <summary>
        /// Obtiene un producto específico por su identificador único.
        /// </summary>
        /// <param name="id">El identificador del producto.</param>
        /// <returns>Una tarea que representa la operación asíncrona, conteniendo el <see cref="Producto"/> si es encontrado; de lo contrario, <c>null</c>.</returns>
        Task<Producto?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene una colección de productos filtrados por el identificador de una categoría específica.
        /// </summary>
        /// <param name="categoryId">El identificador de la categoría por la cual filtrar.</param>
        /// <returns>Una tarea que representa la operación asíncrona, conteniendo una colección de <see cref="Producto"/>.</returns>
        Task<IEnumerable<Producto>> GetByCategoryIdAsync(int categoryId);

        // Métodos de administración (CRUD)

        /// <summary>
        /// Añade un nuevo producto a la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="producto">El objeto <see cref="Producto"/> a añadir.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AddAsync(Producto producto);

        /// <summary>
        /// Actualiza un producto existente en la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="producto">El objeto <see cref="Producto"/> con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task UpdateAsync(Producto producto);

        /// <summary>
        /// Elimina un producto de la base de datos por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del producto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task DeleteAsync(int id);
    }
}
