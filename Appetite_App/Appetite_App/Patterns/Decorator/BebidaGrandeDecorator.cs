using Appetite_App.Models; // Necesario para IProductoComponente
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Decorator
{
    /// <summary>
    /// Implementa un Decorador Concreto que modifica un producto (ej. una bebida) para cambiar su tamaño a "Grande".
    /// Esta clase es un <c>ConcreteDecorator</c> en el Patrón Decorator que incrementa el costo y actualiza la descripción.
    /// </summary>
    public class BebidaGrandeDecorator : ProductoDecorator
    {
        private const decimal CostoExtra = 11500.00M;
        private const string DescripcionExtra = ", Tamaño Grande (+$11.500)";

        /// <summary>
        /// Inicializa una nueva instancia del <see cref="BebidaGrandeDecorator"/>.
        /// </summary>
        /// <param name="componente">El <see cref="IProductoComponente"/> que se va a decorar.</param>
        public BebidaGrandeDecorator(IProductoComponente componente) : base(componente) { }

        /// <summary>
        /// Obtiene la descripción del producto acumulada y le añade la descripción del tamaño "Grande".
        /// </summary>
        /// <returns>La descripción total del producto, incluyendo los extras previos y el tamaño grande.</returns>
        public override string GetDescripcion()
        {
            // Nota: La coma se añade para que se separe correctamente de la descripción previa (ej. "Coca Cola, Tamaño Grande...")
            return _componente.GetDescripcion() + DescripcionExtra;
        }

        /// <summary>
        /// Obtiene el precio acumulado del producto y le añade el costo fijo de la mejora a "Grande".
        /// </summary>
        /// <returns>El precio total, incluyendo el costo base y todos los decoradores aplicados.</returns>
        public override decimal GetPrecio()
        {
            return _componente.GetPrecio() + CostoExtra;
        }
    }
}
