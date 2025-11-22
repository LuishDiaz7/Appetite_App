using Appetite_App.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Observer
{
    /// <summary>
    /// Implementa un Observador Concreto (<c>Concrete Observer</c>) responsable de la trazabilidad y la auditoría.
    /// Este componente registra los eventos de cambio de estado de la orden en un log (simulado por Console.WriteLine).
    /// </summary>
    public class AuditorObserver : IOrderObserver
    {
        // En una aplicación real, aquí se inyectaría un IAuditingService o ILogger.

        /// <summary>
        /// Implementación requerida por <see cref="IOrderObserver"/>.
        /// Reacciona a los eventos de la orden para registrar un evento de auditoría.
        /// (Implementación de <see cref="IOrderObserver.Update"/>).
        /// </summary>
        /// <param name="order">La <see cref="PreOrden"/> que generó el evento.</param>
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
                    Console.WriteLine($"[AUDITORÍA]: ⚠️ Evento de orden desconocido/no manejado: {eventType} para la orden {order.IdOrden}.");
                    break;
            }
        }

        /// <summary>
        /// Registra un evento de auditoría cuando se crea una nueva orden.
        /// </summary>
        /// <param name="orden">La <see cref="PreOrden"/> recién creada.</param>
        private void OnOrderCreated(PreOrden orden)
        {
            Console.WriteLine($"[AUDITORÍA]: 📜 Registro de creación de orden ID: {orden.IdOrden} por usuario {orden.IdUsuario}. Total: {orden.Total:C}.");
        }

        /// <summary>
        /// Registra un evento de auditoría cuando la orden ha sido marcada como preparada.
        /// </summary>
        /// <param name="orden">La <see cref="PreOrden"/> preparada.</param>
        private void OnOrderPrepared(PreOrden orden)
        {
            Console.WriteLine($"[AUDITORÍA]: 📜 Registro de preparación finalizada de orden ID: {orden.IdOrden}.");
        }

        /// <summary>
        /// Registra un evento de auditoría cuando la orden ha sido cancelada.
        /// </summary>
        /// <param name="orden">La <see cref="PreOrden"/> cancelada.</param>
        private void OnOrderCanceled(PreOrden orden)
        {
            // El estado de la orden (ej: "Cancelada por Cliente") podría usarse aquí.
            Console.WriteLine($"[AUDITORÍA]: 📜 Registro de cancelación de orden ID: {orden.IdOrden}. Estado final: {orden.Estado}.");
        }
    }
}