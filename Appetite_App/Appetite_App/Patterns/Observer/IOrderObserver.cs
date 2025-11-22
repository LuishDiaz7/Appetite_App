using Appetite_App.Models;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Observer
{
    /// <summary>
    /// Define la interfaz del Observador (<c>Observer</c>) en el Patrón Observer .
    /// Cualquier clase que implemente esta interfaz podrá suscribirse a un <see cref="IOrderSubject"/>
    /// y será notificada cuando ocurra un evento relevante, como un cambio de estado de la orden.
    /// </summary>
    public interface IOrderObserver
    {
        /// <summary>
        /// Método de actualización (el core del patrón Observer) que se llama cuando el Sujeto notifica un cambio.
        /// La implementación concreta de este método contendrá la lógica de negocio a ejecutar.
        /// </summary>
        /// <param name="order">El objeto <see cref="PreOrden"/> actualizado, con su nuevo estado.</param>
        /// <param name="eventType">El tipo de evento que disparó la notificación (ej. "CREATED", "PREPARED").</param>
        void Update(PreOrden order, string eventType);
    }
}
