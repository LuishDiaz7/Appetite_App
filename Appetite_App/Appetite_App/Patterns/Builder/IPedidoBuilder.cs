using Appetite_App.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Builder
{
    /// <summary>
    /// Define la interfaz del constructor (<c>Builder</c>) para ensamblar el objeto complejo <see cref="PreOrden"/>.
    /// Los métodos permiten la construcción paso a paso de las partes de la orden.
    /// </summary>
    public interface IPedidoBuilder
    {
        /// <summary>
        /// Restablece el constructor, limpiando cualquier producto en construcción y preparando una nueva instancia vacía.
        /// Este método no está presente en el código provisto, pero es una buena práctica en el patrón Builder.
        /// </summary>
        void Reset(); // Añadido para completar la práctica del patrón

        /// <summary>
        /// Define el paso para establecer el <see cref="Usuario"/> que realiza el pedido.
        /// </summary>
        /// <param name="usuario">El objeto <see cref="Usuario"/> que se asociará a la orden.</param>
        void SetUsuario(Usuario usuario);

        /// <summary>
        /// Define el paso para establecer la dirección de entrega del pedido.
        /// </summary>
        /// <param name="direccion">La dirección de entrega.</param>
        void SetDireccion(string direccion);

        /// <summary>
        /// Define el paso para establecer la fecha de creación del pedido.
        /// </summary>
        /// <param name="fecha">La fecha y hora de la orden.</param>
        void SetFecha(DateTime fecha);

        /// <summary>
        /// Define el paso para añadir un ítem (<see cref="DetalleOrden"/>) a la colección de la orden.
        /// </summary>
        /// <param name="detalle">El ítem individual de la orden a agregar.</param>
        void AddDetalle(DetalleOrden detalle);

        /// <summary>
        /// Define el paso final para calcular el costo total de la orden basándose en los detalles añadidos.
        /// </summary>
        void CalcularTotal();

        /// <summary>
        /// Define el paso para obtener el resultado final construido (<see cref="PreOrden"/>).
        /// </summary>
        /// <returns>La <see cref="PreOrden"/> ensamblada.</returns>
        PreOrden GetPreOrden();
    }
}
