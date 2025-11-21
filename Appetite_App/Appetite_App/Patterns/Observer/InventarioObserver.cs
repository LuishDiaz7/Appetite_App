using Appetite_App.Models;
using Microsoft.Extensions.Logging;

namespace Appetite_App.Patterns.Observer
{

    public class InventarioObserver : IOrderObserver
    {
        private readonly ILogger<InventarioObserver> _logger;

        // Se inyectan las dependencias necesarias, como un servicio de inventario o un logger
        public InventarioObserver(ILogger<InventarioObserver> logger)
        {
            _logger = logger;
        }

        public void Update(PreOrden order, string eventType)
        {
            if (eventType == "ORDER_CREATED")
            {
                // Lógica real: Descontar productos del inventario
                _logger.LogInformation($"[INVENTARIO] Descontando inventario para Orden #{order.IdOrden}.");

                foreach (var detalle in order.Detalles)
                {
                    // Simulación del descuento
                    _logger.LogInformation($"- Descontado {detalle.Cantidad} x {detalle.Producto.Nombre}");
                }

                // Si el inventario falla, podría notificar al sujeto de vuelta o generar una excepción.
            }
        }
    }
}
