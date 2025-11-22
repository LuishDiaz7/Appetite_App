using Appetite_App.Models;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Observer
{
    /// <summary>
    /// Implementa un Observador Concreto (<c>Concrete Observer</c>) que reacciona a los cambios en el estado de una orden.
    /// Su responsabilidad principal es gestionar el inventario, descontando los ítems cuando una orden es creada.
    /// </summary>
    public class InventarioObserver : IOrderObserver
    {
        private readonly ILogger<InventarioObserver> _logger;
        // Nota: En un entorno real, aquí se inyectaría un IInventoryService.

        /// <summary>
        /// Inicializa una nueva instancia del <see cref="InventarioObserver"/>.
        /// </summary>
        /// <param name="logger">El registrador (Logger) inyectado para fines de monitoreo y depuración.</param>
        public InventarioObserver(ILogger<InventarioObserver> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Método llamado por el Sujeto (<see cref="IOrderSubject"/>) para notificar un cambio de estado en la orden.
        /// Este observador solo actúa cuando la orden ha sido creada.
        /// (Implementación de <see cref="IOrderObserver.Update"/>).
        /// </summary>
        /// <param name="order">La <see cref="PreOrden"/> que disparó el evento.</param>
        /// <param name="eventType">El tipo de evento (ej. "CREATED").</param>
        public void Update(PreOrden order, string eventType)
        {
            // Solo actuamos si se crea una nueva orden.
            if (eventType == "ORDER_CREATED")
            {
                // Lógica real: Descontar productos del inventario
                _logger.LogInformation($"[INVENTARIO] 📦 Descontando inventario para Orden #{order.IdOrden}.");

                if (order.Detalles == null)
                {
                    _logger.LogError($"[INVENTARIO] La orden #{order.IdOrden} no tiene detalles de producto.");
                    return;
                }

                foreach (var detalle in order.Detalles)
                {
                    // Simulación del descuento: se registraría la llamada al servicio de inventario aquí.
                    // Idealmente, se descontaría el detalle.Producto.Id y detalle.Cantidad.
                    _logger.LogInformation($"- Descontado {detalle.Cantidad} x Producto ID: {detalle.IdProducto} (Item: {detalle.Producto?.Nombre ?? "N/A"})");
                }

                // Consideración de Negocio: Si el descuento de inventario falla,
                // este observador podría lanzar una excepción o llamar a un servicio
                // para notificar al Sujeto de vuelta para revertir la orden a "Pendiente de Inventario".
            }
            // Podríamos añadir lógica para reabastecimiento si eventType fuera "CANCELED".
        }
    }
}
