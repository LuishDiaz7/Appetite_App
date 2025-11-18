using Appetite_App.Models;

namespace Appetite_App.Patterns.Builder
{
    // Director en el diagrama
    public class Director
    {
        private IPedidoBuilder? _builder;

        public IPedidoBuilder Builder
        {
            set { _builder = value; }
        }

        // Método para construir una orden completa
        public PreOrden ConstruirOrden(Usuario usuario, string direccion, List<DetalleOrden> detalles)
        {
            if (_builder == null)
            {
                throw new InvalidOperationException("El Builder no ha sido asignado.");
            }

            _builder.SetUsuario(usuario);
            _builder.SetDireccion(direccion);
            _builder.SetFecha(DateTime.Now);

            foreach (var detalle in detalles)
            {
                _builder.AddDetalle(detalle);
            }

            _builder.CalcularTotal();
            return _builder.GetPreOrden();
        }
    }
}
