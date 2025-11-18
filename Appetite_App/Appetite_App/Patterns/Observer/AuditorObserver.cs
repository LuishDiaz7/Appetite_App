using Appetite_App.Models;

namespace Appetite_App.Patterns.Observer
{
    // AuditorObserver en el diagrama
    public class AuditorObserver : IOrderObserver
    {
        public void OnOrderCreated(PreOrden orden)
        {
            Console.WriteLine($"[AUDITORÍA]: Registro de creación de orden ID: {orden.IdOrden} por usuario {orden.IdUsuario}.");
        }

        public void OnOrderPrepared(PreOrden orden)
        {
            Console.WriteLine($"[AUDITORÍA]: Registro de preparación finalizada de orden ID: {orden.IdOrden}.");
        }

        public void OnOrderCanceled(PreOrden orden)
        {
            Console.WriteLine($"[AUDITORÍA]: Registro de cancelación de orden ID: {orden.IdOrden}. Motivo: {orden.Estado}.");
        }
    }
}