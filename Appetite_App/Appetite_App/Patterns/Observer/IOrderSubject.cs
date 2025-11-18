using Appetite_App.Models;

namespace Appetite_App.Patterns.Observer
{
    // Parte de OrdenSubject en el diagrama
    public interface IOrderSubject
    {
        void Attach(IOrderObserver observer);
        void Detach(IOrderObserver observer);
        void Notify(PreOrden orden, string eventType);
    }
}
