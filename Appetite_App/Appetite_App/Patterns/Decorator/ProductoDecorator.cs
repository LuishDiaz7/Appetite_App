using Appetite_App.Models; // Necesario para IProductoComponente
using System;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Decorator
{
    /// <summary>
    /// Clase abstracta base para todos los decoradores de productos.
    /// Esta clase es el <c>Decorator</c> abstracto en el Patrón Decorator,
    /// y asegura que todos los decoradores puedan envolver a otro <see cref="IProductoComponente"/>
    /// y delegar sus métodos.
    /// </summary>
    public abstract class ProductoDecorator : IProductoComponente
    {
        /// <summary>
        /// El componente que está siendo envuelto o decorado. Puede ser un <see cref="ProductoConcreto"/>
        /// o un <see cref="ProductoDecorator"/> ya existente.
        /// </summary>
        protected IProductoComponente _componente;

        /// <summary>
        /// Inicializa una nueva instancia del <see cref="ProductoDecorator"/>.
        /// </summary>
        /// <param name="componente">El componente que se va a decorar (ej. el producto base o un decorador previo).</param>
        /// <exception cref="ArgumentNullException">Se lanza si el componente proporcionado es nulo.</exception>
        public ProductoDecorator(IProductoComponente componente)
        {
            _componente = componente ?? throw new ArgumentNullException(nameof(componente));
        }

        /// <summary>
        /// Obtiene la descripción delegando la llamada al componente envuelto.
        /// Este método es virtual para que los decoradores concretos puedan sobrescribirlo
        /// y añadir su propia descripción.
        /// </summary>
        /// <returns>La descripción actual del componente.</returns>
        public virtual string GetDescripcion()
        {
            return _componente.GetDescripcion();
        }

        /// <summary>
        /// Obtiene el precio delegando la llamada al componente envuelto.
        /// Este método es virtual para que los decoradores concretos puedan sobrescribirlo
        /// y añadir su propio costo al precio acumulado.
        /// </summary>
        /// <returns>El precio acumulado del componente.</returns>
        public virtual decimal GetPrecio()
        {
            return _componente.GetPrecio();
        }
    }
}
