using Appetite_App.Models;
using System; // Necesario para Console.WriteLine

namespace Appetite_App.Patterns.Observer
{
    // AuditorObserver implementa correctamente IOrderObserver
    public class AuditorObserver : IOrderObserver
    {
        // En una aplicación real, aquí inyectarías un servicio de Logging o una conexión a la base de datos de Auditoría.

        /// <summary>
        /// Implementación requerida por IOrderObserver.
        /// Reacciona a los eventos de la orden para registrar un evento de auditoría.
        /// </summary>
        /// <param name="order">La PreOrden que generó el evento.</param>
        /// <param name="eventType">El tipo de evento (ej: "CREATED", "CANCELED").</param>
        public void Update(PreOrden order, string eventType)
        {
            switch (eventType)
            {
                case "CREATED":
                    OnOrderCreated(order);
                    break;
                case "PREPARED":
                    OnOrderPrepared(order);
                    break;
                case "CANCELED":
                    OnOrderCanceled(order);
                    break;
                default:
                    // Es buena práctica registrar cualquier otro estado que pase por el sistema.
                    Console.WriteLine($"[AUDITORÍA]: Evento de orden desconocido/no manejado: {eventType} para la orden {order.IdOrden}.");
                    break;
            }
        }

        // Métodos privados que contienen la lógica de auditoría específica.

        private void OnOrderCreated(PreOrden orden)
        {
            Console.WriteLine($"[AUDITORÍA]: Registro de creación de orden ID: {orden.IdOrden} por usuario {orden.IdUsuario}.");
        }

        private void OnOrderPrepared(PreOrden orden)
        {
            Console.WriteLine($"[AUDITORÍA]: Registro de preparación finalizada de orden ID: {orden.IdOrden}.");
        }

        private void OnOrderCanceled(PreOrden orden)
        {
            // El estado de la orden podría usarse como motivo de cancelación si no se pasa explícitamente.
            Console.WriteLine($"[AUDITORÍA]: Registro de cancelación de orden ID: {orden.IdOrden}. Motivo: {orden.Estado}.");
        }
    }
}