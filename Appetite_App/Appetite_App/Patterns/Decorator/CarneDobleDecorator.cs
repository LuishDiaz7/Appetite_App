using Appetite_App.Models; // Necesario para IProductoComponente
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Decorator
{
    /// <summary>
    /// Implementa un Decorador Concreto que añade el modificador "Carne Doble" al producto base.
    /// Esta clase incrementa el costo y actualiza la descripción del componente envuelto.
    /// Es un <c>ConcreteDecorator</c> en el Patrón Decorator.
    /// </summary>
    public class CarneDobleDecorator : ProductoDecorator
    {
        private const decimal CostoExtra = 8000.00M;
        private const string DescripcionExtra = ", Carne Doble (+$8.000)";

        /// <summary>
        /// Inicializa una nueva instancia del <see cref="CarneDobleDecorator"/>.
        /// </summary>
        /// <param name="componente">El <see cref="IProductoComponente"/> que se va a decorar (el producto base o un decorador ya aplicado).</param>
        public CarneDobleDecorator(IProductoComponente componente) : base(componente) { }

        /// <summary>
        /// Obtiene la descripción del producto acumulada y le añade la descripción de "Carne Doble".
        /// </summary>
        /// <returns>La descripción total del producto, incluyendo los extras previos y la carne doble.</returns>
        public override string GetDescripcion()
        {
            return _componente.GetDescripcion() + DescripcionExtra;
        }

        /// <summary>
        /// Obtiene el precio acumulado del producto y le añade el costo fijo de la "Carne Doble".
        /// </summary>
        /// <returns>El precio total, incluyendo el costo base y todos los decoradores aplicados.</returns>
        public override decimal GetPrecio()
        {
            return _componente.GetPrecio() + CostoExtra;
        }
    }
}
