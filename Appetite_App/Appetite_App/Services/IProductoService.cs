using Appetite_App.Models;
using Appetite_App.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Appetite_App.Services
{
    /// <summary>
    /// Interfaz que define el contrato de servicio para todas las operaciones relacionadas con la entidad <see cref="Producto"/>.
    /// Este servicio actúa como mediador entre los controladores y la capa de acceso a datos (repositorio).
    /// </summary>
    public interface IProductoService
    {
        // Métodos de Lectura

        /// <summary>
        /// Obtiene una lista de todos los productos disponibles en la base de datos de manera asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona, conteniendo una colección de <see cref="Producto"/>.</returns>
        Task<IEnumerable<Producto>> GetAllProductosAsync();

        /// <summary>
        /// Obtiene un producto específico por su identificador único de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del producto a buscar.</param>
        /// <returns>Una tarea que representa la operación asíncrona, conteniendo el <see cref="Producto"/> si se encuentra, o <c>null</c> si no existe.</returns>
        Task<Producto?> GetProductoByIdAsync(int id);

        // Métodos de Administración (CRUD)

        /// <summary>
        /// Agrega un nuevo producto a la base de datos de manera asíncrona.
        /// </summary>
        /// <param name="producto">El objeto <see cref="Producto"/> a añadir.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AddProductoAsync(Producto producto);    // NUEVO: Crear

        /// <summary>
        /// Actualiza un producto existente en la base de datos de manera asíncrona.
        /// </summary>
        /// <param name="producto">El objeto <see cref="Producto"/> con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task UpdateProductoAsync(Producto producto); // NUEVO: Editar

        /// <summary>
        /// Elimina un producto de la base de datos por su identificador de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del producto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task DeleteProductoAsync(int id);            // NUEVO: Eliminar
    }
}
