using Microsoft.EntityFrameworkCore;
using Appetite_App.Data;
using Appetite_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appetite_App.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio para la entidad <see cref="PreOrden"/>.
    /// Utiliza <see cref="AppetiteContext"/> y Entity Framework Core para la persistencia de datos.
    /// Sigue el Patrón Repositorio y aísla el código de EF Core.
    /// </summary>
    public class OrdenRepository : IOrdenRepository
    {
        private readonly AppetiteContext _context;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de órdenes.
        /// </summary>
        /// <param name="context">El contexto de la base de datos de la aplicación, inyectado vía DI.</param>
        public OrdenRepository(AppetiteContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene de forma asíncrona una colección de todas las órdenes registradas, incluyendo la información del <see cref="Usuario"/> que la realizó.
        /// (Implementación de <see cref="IOrdenRepository.GetAllAsync"/>).
        /// </summary>
        /// <returns>Una tarea que devuelve todas las órdenes con su respectivo usuario.</returns>
        public async Task<IEnumerable<PreOrden>> GetAllAsync()
        {
            // Eager Loading: Incluye el Usuario asociado a la orden.
            return await _context.Ordenes.Include(o => o.Usuario).ToListAsync();
        }

        /// <summary>
        /// Obtiene una orden específica por su identificador único, incluyendo sus <see cref="DetalleOrden"/> asociados.
        /// (Implementación de <see cref="IOrdenRepository.GetByIdAsync"/>).
        /// </summary>
        /// <param name="id">El identificador de la orden.</param>
        /// <returns>Una tarea que devuelve la orden completa si es encontrada; de lo contrario, <c>null</c>.</returns>
        public async Task<PreOrden?> GetByIdAsync(int id)
        {
            // Eager Loading: Incluye los detalles de la orden (los ítems comprados).
            return await _context.Ordenes
                                 .Include(o => o.Detalles)
                                 .FirstOrDefaultAsync(o => o.IdOrden == id);
        }

        /// <summary>
        /// Agrega una nueva orden a la base de datos y persiste los cambios de forma asíncrona.
        /// (Implementación de <see cref="IOrdenRepository.AddAsync"/>).
        /// </summary>
        /// <param name="orden">El objeto <see cref="PreOrden"/> a añadir.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task AddAsync(PreOrden orden)
        {
            _context.Ordenes.Add(orden);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza una orden existente en la base de datos y persiste los cambios de forma asíncrona.
        /// (Implementación de <see cref="IOrdenRepository.UpdateAsync"/>).
        /// </summary>
        /// <param name="orden">El objeto <see cref="PreOrden"/> con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task UpdateAsync(PreOrden orden)
        {
            // Usar Update() para marcar el objeto como modificado.
            _context.Ordenes.Update(orden);
            await _context.SaveChangesAsync();
        }
    }
}
