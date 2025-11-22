using Appetite_App.Models; // Necesario para IProductoComponente
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Decorator
{
    /// <summary>
    /// Implementa un Decorador Concreto que añade el modificador "Queso Extra" al producto base.
    /// Esta clase añade un costo fijo y actualiza la descripción del componente envuelto.
    /// </summary>
    public class QuesoExtraDecorator : ProductoDecorator
    {
        private const decimal CostoExtra = 4000.00M;
        private const string DescripcionExtra = ", Queso Extra (+$4.000)";

        /// <summary>
        /// Inicializa una nueva instancia del <see cref="QuesoExtraDecorator"/>.
        /// </summary>
        /// <param name="componente">El <see cref="IProductoComponente"/> que se va a decorar, que puede ser el producto base o un decorador ya existente.</param>
        public QuesoExtraDecorator(IProductoComponente componente) : base(componente) { }

        /// <summary>
        /// Obtiene la descripción del producto acumulada y le añade la descripción de "Queso Extra".
        /// </summary>
        /// <returns>La descripción total del producto, incluyendo los extras previos y el queso extra.</returns>
        public override string GetDescripcion()
        {
            return _componente.GetDescripcion() + DescripcionExtra;
        }

        /// <summary>
        /// Obtiene el precio acumulado del producto y le añade el costo fijo del "Queso Extra".
        /// </summary>
        /// <returns>El precio total, incluyendo el costo base y todos los decoradores aplicados.</returns>
        public override decimal GetPrecio()
        {
            return _componente.GetPrecio() + CostoExtra;
        }
    }
}
