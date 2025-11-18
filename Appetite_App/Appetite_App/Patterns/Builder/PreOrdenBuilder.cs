using Appetite_App.Models;

namespace Appetite_App.Patterns.Builder
{
    // PreOrdenBuilder en el diagrama
    public class PreOrdenBuilder : IPedidoBuilder
    {
        private PreOrden _preOrden;

        public PreOrdenBuilder()
        {
            this.Reset();
        }

        public void Reset()
        {
            this._preOrden = new PreOrden();
            this._preOrden.Detalles = new List<DetalleOrden>();
            this._preOrden.Estado = "Pendiente";
        }

        public void SetUsuario(Usuario usuario)
        {
            this._preOrden.IdUsuario = usuario.IdUsuario;
            this._preOrden.Usuario = usuario;
        }

        public void SetDireccion(string direccion)
        {
            this._preOrden.Direccion = direccion;
        }

        public void SetFecha(DateTime fecha)
        {
            this._preOrden.Fecha = fecha;
        }

        public void AddDetalle(DetalleOrden detalle)
        {
            this._preOrden.Detalles.Add(detalle);
        }

        public void CalcularTotal()
        {
            decimal total = 0;
            foreach (var detalle in this._preOrden.Detalles)
            {
                total += detalle.Subtotal;
            }
            this._preOrden.Total = total;
        }

        public PreOrden GetPreOrden()
        {
            PreOrden resultado = this._preOrden;
            this.Reset(); // Resetear para construir una nueva orden
            return resultado;
        }
    }
}
