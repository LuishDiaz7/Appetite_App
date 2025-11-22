using Appetite_App.Models;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Observer
{
    /// <summary>
    /// Implementación concreta del Sujeto (<c>Concrete Subject</c>) en el Patrón Observer.
    /// Mantiene la lista de observadores suscritos (<see cref="IOrderObserver"/>) y
    /// es responsable de notificarles sobre los cambios de estado en la orden.
    /// </summary>
    public class OrderSubject : IOrderSubject
    {
        // La lista interna de suscriptores (Observadores)
        private List<IOrderObserver> _observers = new List<IOrderObserver>();

        /// <summary>
        /// Agrega un observador a la lista de suscritos para recibir notificaciones.
        /// (Implementación de <see cref="IOrderSubject.Attach"/>).
        /// </summary>
        /// <param name="observer">El observador a suscribir.</param>
        /// <exception cref="ArgumentNullException">Se lanza si el observador es nulo.</exception>
        public void Attach(IOrderObserver observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                // Console.WriteLine($"Observer {observer.GetType().Name} attached."); // Opcional: log de suscripción
            }
        }

        /// <summary>
        /// Elimina un observador de la lista de suscritos.
        /// (Implementación de <see cref="IOrderSubject.Detach"/>).
        /// </summary>
        /// <param name="observer">El observador a desuscribir.</param>
        public void Detach(IOrderObserver observer)
        {
            if (observer != null)
            {
                _observers.Remove(observer);
                // Console.WriteLine($"Observer {observer.GetType().Name} detached."); // Opcional: log de desuscripción
            }
        }

        /// <summary>
        /// Recorre la lista de observadores y llama a su método <c>Update</c>,
        /// informándoles sobre el cambio de estado de la orden.
        /// (Implementación de <see cref="IOrderSubject.Notify"/>).
        /// </summary>
        /// <param name="order">La <see cref="PreOrden"/> que ha cambiado.</param>
        /// <param name="eventType">El tipo de evento (ej. "CREATED").</param>
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
