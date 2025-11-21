using Appetite_App.Models;
using System;

namespace Appetite_App.Patterns.Builder
{
    public interface IPedidoBuilder
    {
        void SetUsuario(Usuario usuario);
        void SetDireccion(string direccion);
        void SetFecha(DateTime fecha);
        void AddDetalle(DetalleOrden detalle); // Usamos DetalleOrden
        void CalcularTotal();
        PreOrden GetPreOrden(); // Devuelve PreOrden
    }
}
