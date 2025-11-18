using Appetite_App.Models;

namespace Appetite_App.Patterns.Observer
{
    // Interface Observer en el diagrama
    public interface IOrderObserver
    {
        void OnOrderCreated(PreOrden orden);
        void OnOrderPrepared(PreOrden orden);
        void OnOrderCanceled(PreOrden orden);
    }
}
