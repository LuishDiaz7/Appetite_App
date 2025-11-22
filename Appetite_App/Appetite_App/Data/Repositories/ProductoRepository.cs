using Appetite_App.Data;
using Appetite_App.Models;
using Appetite_App.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Appetite_App.Data.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio para la entidad <see cref="Producto"/>.
    /// Utiliza <see cref="AppetiteContext"/> y Entity Framework Core para la persistencia de datos.
    /// Sigue el Patrón Repositorio y aísla el código de EF Core.
    /// </summary>
    public class ProductoRepository : IProductoRepository
    {
        private readonly AppetiteContext _context;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio.
        /// </summary>
        /// <param name="context">El contexto de la base de datos de la aplicación, inyectado vía DI.</param>
        public ProductoRepository(AppetiteContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene de forma asíncrona una colección de todos los productos disponibles en la base de datos.
        /// (Implementación de <see cref="IProductoRepository.GetAllAsync"/>).
        /// </summary>
        /// <returns>Una tarea que devuelve todos los productos.</returns>
        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _context.Productos.ToListAsync();
        }

        /// <summary>
        /// Obtiene de forma asíncrona una colección de todos los productos, incluyendo la información de su categoría asociada.
        /// Utiliza <c>Include</c> para realizar la carga ansiosa (Eager Loading).
        /// (Implementación de <see cref="IProductoRepository.GetAllWithCategoryAsync"/>).
        /// </summary>
        /// <returns>Una tarea que devuelve todos los productos con su categoría.</returns>
        public async Task<IEnumerable<Producto>> GetAllWithCategoryAsync()
        {
            // Usamos .Include() para cargar la Categoría junto con el Producto
            return await _context.Productos
                .Include(p => p.Categoria)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene un producto específico por su identificador único de forma asíncrona.
        /// (Implementación de <see cref="IProductoRepository.GetByIdAsync"/>).
        /// </summary>
        /// <param name="id">El identificador del producto.</param>
        /// <returns>Una tarea que devuelve el producto si es encontrado; de lo contrario, <c>null</c>.</returns>
        public async Task<Producto?> GetByIdAsync(int id)
        {
            // FindAsync busca primero en la caché de Entity Framework.
            return await _context.Productos.FindAsync(id);
        }

        /// <summary>
        /// Obtiene una colección de productos filtrados por el identificador de una categoría específica de forma asíncrona.
        /// (Implementación de <see cref="IProductoRepository.GetByCategoryIdAsync"/>).
        /// </summary>
        /// <param name="categoryId">El identificador de la categoría.</param>
        /// <returns>Una tarea que devuelve la colección de productos de la categoría especificada.</returns>
        public async Task<IEnumerable<Producto>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Productos
                    // Filtra por la clave foránea
                    .Where(p => p.IdCategoria == categoryId)
                    .Include(p => p.Categoria) // Carga la categoría
                    .ToListAsync();
        }

        /// <summary>
        /// Agrega un nuevo producto a la base de datos y persiste los cambios de forma asíncrona.
        /// (Implementación de <see cref="IProductoRepository.AddAsync"/>).
        /// </summary>
        /// <param name="producto">El objeto <see cref="Producto"/> a añadir.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task AddAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

        }

        /// <summary>
        /// Actualiza un producto existente en la base de datos y persiste los cambios de forma asíncrona.
        /// (Implementación de <see cref="IProductoRepository.UpdateAsync"/>).
        /// </summary>
        /// <param name="producto">El objeto <see cref="Producto"/> con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task UpdateAsync(Producto producto)
        {
            // Marcar el estado como modificado, lo cual es útil si el objeto viene desconectado del contexto.
            _context.Entry(producto).State = EntityState.Modified;

            // Persistir los cambios
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina un producto por su identificador único de la base de datos de forma asíncrona.
        /// (Implementación de <see cref="IProductoRepository.DeleteAsync"/>).
        /// </summary>
        /// <param name="id">El identificador del producto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task DeleteAsync(int id)
        {
            // Buscar el producto por ID
            Producto? producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
        }
    }
}