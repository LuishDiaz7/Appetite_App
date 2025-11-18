using Appetite_App.Models;

namespace Appetite_App.Services // Mismo namespace que el servicio
{
    public interface IProductoService
    {
        Task<IEnumerable<Producto>> GetAllProductosAsync();
    }
}
