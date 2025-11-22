using Appetite_App.Models;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Observer
{
    /// <summary>
    /// Define la interfaz del Sujeto (<c>Subject</c>) en el Patrón Observer .
    /// El Sujeto (en este caso, la gestión de órdenes) mantiene una lista de dependientes (<c>IOrderObserver</c>)
    /// y les notifica automáticamente cualquier cambio de estado relevante.
    /// </summary>
    public interface IOrderSubject
    {
        /// <summary>
        /// Registra un nuevo observador en la lista de suscritos.
        /// </summary>
        /// <param name="observer">El observador (<see cref="IOrderObserver"/>) que desea ser notificado.</param>
        void Attach(IOrderObserver observer);

        /// <summary>
        /// Elimina un observador de la lista de suscritos.
        /// </summary>
        /// <param name="observer">El observador (<see cref="IOrderObserver"/>) que ya no desea ser notificado.</param>
        void Detach(IOrderObserver observer);

        /// <summary>
        /// Notifica a todos los observadores suscritos sobre un cambio en el estado de la orden.
        /// </summary>
        /// <param name="order">El objeto <see cref="PreOrden"/> que ha cambiado de estado.</param>
        /// <param name="eventType">Una cadena que describe el tipo de evento ocurrido (ej. "CREATED", "PREPARED").</param>
        void Notify(PreOrden order, string eventType);
    }
}
