using Appetite_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Builder
{
    /// <summary>
    /// Implementa el constructor concreto (<c>ConcreteBuilder</c>) para la entidad <see cref="PreOrden"/>.
    /// Contiene la lógica para ensamblar la orden paso a paso, calcular el total y restablecer el estado interno.
    /// </summary>
    public class PreOrdenBuilder : IPedidoBuilder // Asumimos esta interfaz
    {
        private PreOrden _preOrden;

        /// <summary>
        /// Inicializa una nueva instancia del <see cref="PreOrdenBuilder"/> y restablece el producto en construcción.
        /// </summary>
        public PreOrdenBuilder()
        {
            Reset();
        }

        /// <summary>
        /// Restablece el constructor, creando una nueva instancia de <see cref="PreOrden"/> con valores predeterminados.
        /// (Implementación de <see cref="IPedidoBuilder.Reset"/>).
        /// </summary>
        public void Reset()
        {
            _preOrden = new PreOrden
            {
                Detalles = new List<DetalleOrden>(),
                Fecha = DateTime.Now,
                Estado = "Pendiente"
            };
        }

        /// <summary>
        /// Establece el usuario que realiza la orden.
        /// (Implementación de <see cref="IPedidoBuilder.SetUsuario"/>).
        /// </summary>
        /// <param name="usuario">El objeto <see cref="Usuario"/> que realiza la compra.</param>
        public void SetUsuario(Usuario usuario)
        {
            _preOrden.Usuario = usuario;
            _preOrden.IdUsuario = usuario.Id; // Asume que Usuario tiene una propiedad Id
        }

        /// <summary>
        /// Establece la dirección de entrega de la orden.
        /// (Implementación de <see cref="IPedidoBuilder.SetDireccion"/>).
        /// </summary>
        /// <param name="direccion">La dirección de entrega en formato string.</param>
        public void SetDireccion(string direccion)
        {
            _preOrden.Direccion = direccion;
        }

        /// <summary>
        /// Establece la fecha de creación de la orden.
        /// (Implementación de <see cref="IPedidoBuilder.SetFecha"/>).
        /// </summary>
        /// <param name="fecha">La fecha de la orden.</param>
        public void SetFecha(DateTime fecha)
        {
            _preOrden.Fecha = fecha;
        }

        /// <summary>
        /// Añade un ítem de orden (<see cref="DetalleOrden"/>) a la colección de detalles de la orden.
        /// (Implementación de <see cref="IPedidoBuilder.AddDetalle"/>).
        /// </summary>
        /// <param name="detalle">El detalle de orden a añadir.</param>
        public void AddDetalle(DetalleOrden detalle)
        {
            _preOrden.Detalles.Add(detalle);
        }

        /// <summary>
        /// Calcula el costo total de la orden sumando los subtotales de todos sus detalles.
        /// (Implementación de <see cref="IPedidoBuilder.CalcularTotal"/>).
        /// </summary>
        public void CalcularTotal()
        {
            _preOrden.Total = _preOrden.Detalles.Sum(d => d.Subtotal);
        }

        /// <summary>
        /// Obtiene el objeto <see cref="PreOrden"/> construido y luego llama a <see cref="Reset()"/> para preparar el constructor para la siguiente orden.
        /// </summary>
        /// <returns>La <see cref="PreOrden"/> ensamblada.</returns>
        public PreOrden GetPreOrden()
        {
            PreOrden resultado = _preOrden;
            Reset(); // Importante: Limpiar el constructor después de devolver el producto.
            return resultado;
        }
    }
}