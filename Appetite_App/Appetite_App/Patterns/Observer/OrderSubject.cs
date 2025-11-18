using Appetite_App.Models;

namespace Appetite_App.Patterns.Observer
{
    // OrdenSubject en el diagrama
    public class OrderSubject : IOrderSubject
    {
        private List<IOrderObserver> _observers = new List<IOrderObserver>();

        public void Attach(IOrderObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Detach(IOrderObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(PreOrden orden, string eventType)
        {
            foreach (var observer in _observers)
            {
                switch (eventType)
                {
                    case "Created":
                        observer.OnOrderCreated(orden);
                        break;
                    case "Prepared":
                        observer.OnOrderPrepared(orden);
                        break;
                    case "Canceled":
                        observer.OnOrderCanceled(orden);
                        break;
                }
            }
        }
    }
}
