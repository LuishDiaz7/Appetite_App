using Appetite_App.Models;

namespace Appetite_App.Patterns.Observer
{
    // La interfaz Sujeto (Subject) define las operaciones para manejar a los observadores.
    public interface IOrderSubject
    {
        // Métodos para manejar a los observadores
        void Attach(IOrderObserver observer);
        void Detach(IOrderObserver observer);

        // Método de notificación, que debe ser llamado por el OrdenService
        void Notify(PreOrden order, string eventType);
    }
}
