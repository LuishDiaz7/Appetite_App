using Appetite_App.Models;
using Appetite_App.Repositories; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appetite_App.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<IEnumerable<Producto>> GetAllProductosAsync()
        {
            // Llama al método especializado que trae la Categoría
            return await _productoRepository.GetAllWithCategoryAsync();
        }
    }
}
