using Appetite_App.Models;
using Appetite_App.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appetite_App.Services
{
    // Asegúrate de que esta clase implemente IProductoService
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        // ---------------------------------------------
        // IMPLEMENTACIÓN DE MÉTODOS DE LECTURA
        // ---------------------------------------------
        public async Task<IEnumerable<Producto>> GetAllProductosAsync()
        {
            // Llama al método especializado que trae la Categoría (asumimos que existe en el Repository)
            return await _productoRepository.GetAllWithCategoryAsync();
        }

        // NUEVO: Implementación de GetProductoByIdAsync
        public async Task<Producto?> GetProductoByIdAsync(int id)
        {
            // Delega la obtención al Repositorio
            return await _productoRepository.GetByIdAsync(id);
        }

        // ---------------------------------------------
        // IMPLEMENTACIÓN DE MÉTODOS DE ESCRITURA (CRUD)
        // ---------------------------------------------

        // NUEVO: Implementación de AddProductoAsync
        public async Task AddProductoAsync(Producto producto)
        {
            // Delega la adición al Repositorio
            await _productoRepository.AddAsync(producto);
        }

        // NUEVO: Implementación de UpdateProductoAsync
        public async Task UpdateProductoAsync(Producto producto)
        {
            // Delega la actualización al Repositorio
            await _productoRepository.UpdateAsync(producto);
        }

        // NUEVO: Implementación de DeleteProductoAsync
        public async Task DeleteProductoAsync(int id)
        {
            // Delega la eliminación al Repositorio
            await _productoRepository.DeleteAsync(id);
        }
    }
}
