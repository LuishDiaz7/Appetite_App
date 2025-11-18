using Appetite_App.Models;

namespace Appetite_App.Patterns.Builder
{
    // PedidoBuilder en el diagrama
    public interface IPedidoBuilder
    {
        void SetUsuario(Usuario usuario);
        void SetDireccion(string direccion);
        void SetFecha(DateTime fecha);
        void AddDetalle(DetalleOrden detalle);
        void CalcularTotal();
        PreOrden GetPreOrden();
    }
}
