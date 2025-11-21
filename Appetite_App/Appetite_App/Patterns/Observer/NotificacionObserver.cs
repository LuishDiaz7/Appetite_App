using Appetite_App.Models;
using System; // Necesario para Console.WriteLine

namespace Appetite_App.Patterns.Observer
{
    // NotificacionObserver implementa correctamente IOrderObserver
    public class NotificacionObserver : IOrderObserver
    {
        // El NotificacionObserver no necesita inyección de dependencias como el Inventario,
        // pero podría inyectarse un ILogger o un servicio de Email/SMS real si fuera necesario.

        /// <summary>
        /// Implementación requerida por IOrderObserver.
        /// Reacciona a los eventos de la orden para enviar notificaciones al usuario.
        /// </summary>
        /// <param name="order">La PreOrden que generó el evento.</param>
        /// <param name="eventType">El tipo de evento (ej: "CREATED", "PREPARED").</param>
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
                // Puedes añadir más casos si tienes otros estados como "DELIVERED", etc.
                default:
                    // Ignorar eventos no manejados
                    break;
            }
        }

        // Métodos privados (o protegidos) que contienen la lógica de notificación específica.
        // Anteriormente eran públicos, pero ahora solo son llamados por Update().

        private void OnOrderCreated(PreOrden orden)
        {
            Console.WriteLine($"[NOTIFICACIÓN]: Enviando email al usuario {orden.IdUsuario}: ¡Tu orden {orden.IdOrden} ha sido recibida!");
        }

        private void OnOrderPrepared(PreOrden orden)
        {
            Console.WriteLine($"[NOTIFICACIÓN]: Enviando SMS al usuario {orden.IdUsuario}: ¡Tu orden {orden.IdOrden} está lista para recoger o en camino!");
        }

        private void OnOrderCanceled(PreOrden orden)
        {
            Console.WriteLine($"[NOTIFICACIÓN]: Enviando email al usuario {orden.IdUsuario}: Tu orden {orden.IdOrden} ha sido cancelada.");
        }
    }
}
