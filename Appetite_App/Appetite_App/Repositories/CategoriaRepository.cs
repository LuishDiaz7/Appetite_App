using Microsoft.EntityFrameworkCore;
using Appetite_App.Data;
using Appetite_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appetite_App.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio para la entidad <see cref="Categoria"/>.
    /// Utiliza <see cref="AppetiteContext"/> y Entity Framework Core para la persistencia de datos.
    /// Sigue el Patrón Repositorio y aísla el código de EF Core.
    /// </summary>
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AppetiteContext _context;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de categorías.
        /// </summary>
        /// <param name="context">El contexto de la base de datos de la aplicación, inyectado vía DI.</param>
        public CategoriaRepository(AppetiteContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene de forma asíncrona una colección de todas las categorías disponibles.
        /// (Implementación de <see cref="ICategoriaRepository.GetAllAsync"/>).
        /// </summary>
        /// <returns>Una tarea que devuelve la lista de categorías.</returns>
        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            return await _context.Categorias.ToListAsync();
        }

        /// <summary>
        /// Obtiene una categoría específica por su identificador único de forma asíncrona.
        /// (Implementación de <see cref="ICategoriaRepository.GetByIdAsync"/>).
        /// </summary>
        /// <param name="id">El identificador de la categoría.</param>
        /// <returns>Una tarea que devuelve la categoría si es encontrada; de lo contrario, <c>null</c>.</returns>
        public async Task<Categoria?> GetByIdAsync(int id)
        {
            return await _context.Categorias.FindAsync(id);
        }

        /// <summary>
        /// Agrega una nueva categoría a la base de datos y persiste los cambios de forma asíncrona.
        /// (Implementación de <see cref="ICategoriaRepository.AddAsync"/>).
        /// </summary>
        /// <param name="categoria">El objeto <see cref="Categoria"/> a añadir.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task AddAsync(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza una categoría existente en la base de datos y persiste los cambios de forma asíncrona.
        /// (Implementación de <see cref="ICategoriaRepository.UpdateAsync"/>).
        /// </summary>
        /// <param name="categoria">El objeto <see cref="Categoria"/> con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task UpdateAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina una categoría de la base de datos por su identificador único de forma asíncrona.
        /// (Implementación de <see cref="ICategoriaRepository.DeleteAsync"/>).
        /// </summary>
        /// <param name="id">El identificador de la categoría a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task DeleteAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Obtiene de forma asíncrona una colección de todas las categorías, incluyendo sus <see cref="Producto"/> asociados.
        /// Utiliza <c>Include</c> para realizar la carga ansiosa (Eager Loading).
        /// (Implementación de <see cref="ICategoriaRepository.GetAllWithProductsAsync"/>).
        /// </summary>
        /// <returns>Una tarea que devuelve la lista de categorías con sus productos.</returns>
        public async Task<IEnumerable<Categoria>> GetAllWithProductsAsync()
        {
            return await _context.Categorias
                                 .Include(c => c.Productos) // Carga los productos asociados
                                 .ToListAsync();
        }

    }
}
