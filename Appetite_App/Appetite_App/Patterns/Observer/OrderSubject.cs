using Appetite_App.Models;
using System.Collections.Generic;

namespace Appetite_App.Patterns.Observer
{

    // El Sujeto concreto que mantiene el estado y notifica
    public class OrderSubject : IOrderSubject
    {
        private List<IOrderObserver> _observers = new List<IOrderObserver>();

        public void Attach(IOrderObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IOrderObserver observer)
        {
            _observers.Remove(observer);
        }

        // El método de notificación que se llama cuando el estado de la orden cambia
        public void Notify(PreOrden order, string eventType)
        {
            // Itera sobre todos los observadores y llama a su método de actualización
            foreach (var observer in _observers)
            {
                observer.Update(order, eventType);
            }
        }
    }
}
