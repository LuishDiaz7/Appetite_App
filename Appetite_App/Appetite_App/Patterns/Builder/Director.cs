using Appetite_App.Models;
using System.Collections.Generic;
using System;

namespace Appetite_App.Patterns.Builder
{
    public class Director
    {
        private IPedidoBuilder _builder;

        // Propiedad para asignar el Builder concreto (es una buena práctica de diseño)
        public IPedidoBuilder Builder
        {
            set { _builder = value; }
        }

        /// <summary>
        /// Define la secuencia para crear una PreOrden completa y válida.
        /// </summary>
        // Nota: ahora usa List<DetalleOrden>
        public void ConstruirPedidoCompleto(Usuario usuario, List<DetalleOrden> detallesCarrito, string direccion)
        {
            if (_builder == null)
            {
                throw new InvalidOperationException("El Builder no ha sido asignado al Director.");
            }

            // Los métodos llamados deben coincidir con IPedidoBuilder
            _builder.SetUsuario(usuario);
            _builder.SetDireccion(direccion);
            _builder.SetFecha(DateTime.Now);

            foreach (var detalle in detallesCarrito)
            {
                _builder.AddDetalle(detalle);
            }

            _builder.CalcularTotal();
        }

        /// <summary>
        /// Obtiene el objeto PreOrden construido por el Builder.
        /// </summary>
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
