using Appetite_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appetite_App.Services
{
    public interface IProductoService
    {
        // Métodos de Lectura
        Task<IEnumerable<Producto>> GetAllProductosAsync();
        Task<Producto?> GetProductoByIdAsync(int id); // NUEVO: Para obtener un solo producto

        // Métodos de Administración (CRUD)
        Task AddProductoAsync(Producto producto);    // NUEVO: Crear
        Task UpdateProductoAsync(Producto producto); // NUEVO: Editar
        Task DeleteProductoAsync(int id);           // NUEVO: Eliminar
    }
}
