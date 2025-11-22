using Appetite_App.Models;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Builder
{
    /// <summary>
    /// Clase que actúa como el <c>Director</c> en el Patrón Builder.
    /// Es responsable de definir la secuencia de pasos necesarios para construir
    /// una <see cref="PreOrden"/> válida y compleja. No crea las partes, sino que
    /// dirige al <see cref="IPedidoBuilder"/> concreto.
    /// </summary>
    public class Director
    {
        private IPedidoBuilder _builder;

        /// <summary>
        /// Obtiene o establece el constructor (<c>Builder</c>) que el Director utilizará.
        /// Permite al Cliente (ej. <see cref="Services.OrdenService"/>) inyectar el <c>ConcreteBuilder</c> deseado.
        /// </summary>
        /// <exception cref="ArgumentNullException">Se lanza si se intenta asignar un constructor nulo.</exception>
        public IPedidoBuilder Builder
        {
            set
            {
                _builder = value ?? throw new ArgumentNullException(nameof(value), "El Builder no puede ser nulo.");
            }
        }

        /// <summary>
        /// Define la secuencia de construcción para crear una <see cref="PreOrden"/> completa y lista para ser persistida.
        /// </summary>
        /// <param name="usuario">El <see cref="Usuario"/> que realiza el pedido.</param>
        /// <param name="detallesCarrito">La lista de ítems de la orden (<see cref="DetalleOrden"/>) ya procesados por el Decorator.</param>
        /// <param name="direccion">La dirección de entrega.</param>
        /// <exception cref="InvalidOperationException">Se lanza si el Builder no ha sido asignado antes de llamar a este método.</exception>
        public void ConstruirPedidoCompleto(Usuario usuario, List<DetalleOrden> detallesCarrito, string direccion)
        {
            if (_builder == null)
            {
                throw new InvalidOperationException("El Builder no ha sido asignado al Director. Asigne un Builder antes de construir.");
            }

            // 1. Establecer datos de la cabecera
            _builder.SetUsuario(usuario);
            _builder.SetDireccion(direccion);
            _builder.SetFecha(DateTime.Now);

            // 2. Añadir detalles
            foreach (var detalle in detallesCarrito)
            {
                _builder.AddDetalle(detalle);
            }

            // 3. Paso final
            _builder.CalcularTotal();
            // No llama a GetPreOrden, eso es responsabilidad del Cliente.
        }

        /// <summary>
        /// Obtiene el objeto <see cref="PreOrden"/> construido por el <c>ConcreteBuilder</c> actual.
        /// </summary>
        /// <returns>La <see cref="PreOrden"/> resultante del proceso de construcción.</returns>
        /// <exception cref="InvalidOperationException">Se lanza si el Builder no ha sido asignado.</exception>
        public PreOrden GetResultado()
        {
            if (_builder == null)
            {
                throw new InvalidOperationException("El Builder no ha sido asignado al Director.");
            }
            // Llama al método final de la interfaz
            return _builder.GetPreOrden();
        }
    }
}
