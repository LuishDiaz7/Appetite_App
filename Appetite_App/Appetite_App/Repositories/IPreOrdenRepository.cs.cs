using System.Collections.Generic;
using System.Threading.Tasks;
using Appetite_App.Models;

namespace Appetite_App.Repositories
{
    /// <summary>
    /// Define el contrato (interfaz) para el repositorio de la entidad <see cref="PreOrden"/> (Órdenes Previas).
    /// Esta interfaz es crucial para la gestión y seguimiento del estado de las órdenes en el sistema.
    /// Sigue el Patrón Repositorio para desacoplar el acceso a datos.
    /// </summary>
    public interface IOrdenRepository // Nota: Para consistencia con el modelo, se esperaría IPreOrdenRepository.
    {
        /// <summary>
        /// Obtiene una colección de todas las órdenes registradas en el sistema de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona, conteniendo una colección de <see cref="PreOrden"/>.</returns>
        Task<IEnumerable<PreOrden>> GetAllAsync();

        /// <summary>
        /// Obtiene una orden específica por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la orden.</param>
        /// <returns>Una tarea que representa la operación asíncrona, conteniendo la <see cref="PreOrden"/> si es encontrada; de lo contrario, <c>null</c>.</returns>
        Task<PreOrden?> GetByIdAsync(int id);

        /// <summary>
        /// Agrega una nueva orden a la base de datos de forma asíncrona.
        /// Este método se utiliza típicamente al finalizar el proceso de compra.
        /// </summary>
        /// <param name="orden">El objeto <see cref="PreOrden"/> a añadir.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AddAsync(PreOrden orden);

        /// <summary>
        /// Actualiza una orden existente en la base de datos de forma asíncrona.
        /// Este método es crucial para cambiar el <see cref="PreOrden.Estado"/> de la orden (Patrón Observer).
        /// </summary>
        /// <param name="orden">El objeto <see cref="PreOrden"/> con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task UpdateAsync(PreOrden orden);
    }
}