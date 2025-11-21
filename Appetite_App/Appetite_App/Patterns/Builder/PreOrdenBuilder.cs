using Appetite_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Appetite_App.Patterns.Builder
{
    public class PreOrdenBuilder : IPedidoBuilder
    {
        private PreOrden _preOrden;

        public PreOrdenBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            _preOrden = new PreOrden
            {
                Detalles = new List<DetalleOrden>(),
                Fecha = DateTime.Now,
                Estado = "Pendiente"
            };
        }

        public void SetUsuario(Usuario usuario)
        {
            _preOrden.Usuario = usuario;
            _preOrden.IdUsuario = usuario.Id; // Asume que Usuario tiene una propiedad Id
        }

        public void SetDireccion(string direccion)
        {
            _preOrden.Direccion = direccion;
        }

        public void SetFecha(DateTime fecha)
        {
            _preOrden.Fecha = fecha;
        }

        public void AddDetalle(DetalleOrden detalle)
        {
            _preOrden.Detalles.Add(detalle);
        }

        public void CalcularTotal()
        {
            // Suma los subtotales de los detalles de orden
            _preOrden.Total = _preOrden.Detalles.Sum(d => d.Subtotal);
        }

        public PreOrden GetPreOrden()
        {
            PreOrden resultado = _preOrden;
            Reset();
            return resultado;
        }
    }
}