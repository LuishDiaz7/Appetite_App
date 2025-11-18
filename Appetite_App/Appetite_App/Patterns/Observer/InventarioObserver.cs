using Appetite_App.Models;
using Appetite_App.Repositories; 

namespace Appetite_App.Patterns.Observer
{
    // InventarioObserver en el diagrama
    public class InventarioObserver : IOrderObserver
    {
        // En un prototipo real, inyectaríamos el IProductoRepository aquí.
        // private readonly IProductoRepository _productoRepository;

        // public InventarioObserver(IProductoRepository repo) { _productoRepository = repo; }

        public void OnOrderCreated(PreOrden orden)
        {
            // Lógica: Reducir el stock de productos
            Console.WriteLine($"[INVENTARIO]: Orden {orden.IdOrden} creada. Descontando stock...");
            // _productoRepository.DecrementStock(orden.Detalles);
        }

        public void OnOrderPrepared(PreOrden orden)
        {
            // No requiere acción.
        }

        public void OnOrderCanceled(PreOrden orden)
        {
            // Lógica: Devolver el stock de productos
            Console.WriteLine($"[INVENTARIO]: Orden {orden.IdOrden} cancelada. Reponiendo stock...");
            // _productoRepository.IncrementStock(orden.Detalles);
        }
    }
}
