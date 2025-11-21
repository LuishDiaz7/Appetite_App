using Appetite_App.Models;

namespace Appetite_App.Patterns.Observer
{
    // Interface Observer en el diagrama
    public interface IOrderObserver
    {
        void Update(PreOrden order, string eventType);
    }
}
