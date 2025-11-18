using Appetite_App.Models;

namespace Appetite_App.Patterns.Observer
{
    // NotificacionObserver en el diagrama
    public class NotificacionObserver : IOrderObserver
    {
        public void OnOrderCreated(PreOrden orden)
        {
            Console.WriteLine($"[NOTIFICACIÓN]: Enviando email al usuario {orden.IdUsuario}: ¡Tu orden {orden.IdOrden} ha sido recibida!");
        }

        public void OnOrderPrepared(PreOrden orden)
        {
            Console.WriteLine($"[NOTIFICACIÓN]: Enviando SMS al usuario {orden.IdUsuario}: ¡Tu orden {orden.IdOrden} está lista para recoger o en camino!");
        }

        public void OnOrderCanceled(PreOrden orden)
        {
            Console.WriteLine($"[NOTIFICACIÓN]: Enviando email al usuario {orden.IdUsuario}: Tu orden {orden.IdOrden} ha sido cancelada.");
        }
    }
}
