using Appetite_App.Models;
using Appetite_App.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Services
{
    /// <summary>
    /// Implementación concreta del servicio <see cref="IProductoService"/>.
    /// Esta clase actúa como la capa de negocio, mediando entre los controladores y
    /// la capa de acceso a datos (<see cref="IProductoRepository"/>), y es responsable
    /// de las operaciones CRUD de la entidad <see cref="Producto"/>.
    /// </summary>
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProductoService"/>.
        /// Inyecta la dependencia de <see cref="IProductoRepository"/> para delegar
        /// las operaciones de persistencia.
        /// </summary>
        /// <param name="productoRepository">El repositorio de productos inyectado.</param>
        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        // ---------------------------------------------
        // IMPLEMENTACIÓN DE MÉTODOS DE LECTURA
        // ---------------------------------------------

        /// <summary>
        /// Obtiene de manera asíncrona todos los productos, incluyendo su categoría asociada.
        /// </summary>
        /// <returns>Una colección de <see cref="Producto"/> con datos de <see cref="Categoria"/> cargados.</returns>
        public async Task<IEnumerable<Producto>> GetAllProductosAsync()
        {
            // Llama al método especializado del Repositorio que realiza el Eager Loading de la Categoría
            return await _productoRepository.GetAllWithCategoryAsync();
        }

        /// <summary>
        /// Obtiene un producto específico por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del producto.</param>
        /// <returns>El <see cref="Producto"/> encontrado o <c>null</c>.</returns>
        public async Task<Producto?> GetProductoByIdAsync(int id)
        {
            // Delega la obtención al Repositorio
            return await _productoRepository.GetByIdAsync(id);
        }

        // ---------------------------------------------
        // IMPLEMENTACIÓN DE MÉTODOS DE ESCRITURA (CRUD)
        // ---------------------------------------------

        /// <summary>
        /// Agrega un nuevo producto a la base de datos de manera asíncrona.
        /// </summary>
        /// <param name="producto">El objeto <see cref="Producto"/> a añadir.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task AddProductoAsync(Producto producto)
        {
            // Delega la adición al Repositorio
            await _productoRepository.AddAsync(producto);
        }

        /// <summary>
        /// Actualiza un producto existente en la base de datos de manera asíncrona.
        /// </summary>
        /// <param name="producto">El objeto <see cref="Producto"/> con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task UpdateProductoAsync(Producto producto)
        {
            // Delega la actualización al Repositorio
            await _productoRepository.UpdateAsync(producto);
        }

        /// <summary>
        /// Elimina un producto de la base de datos por su identificador de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del producto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task DeleteProductoAsync(int id)
        {
            // Delega la eliminación al Repositorio
            await _productoRepository.DeleteAsync(id);
        }
    }
}
