using Appetite_App.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Decorator
{
    /// <summary>
    /// Representa el componente base sin decorar en el Patrón Decorator.
    /// Esta clase encapsula el modelo de dominio <see cref="Producto"/> y proporciona
    /// el precio y la descripción iniciales a la cadena de decoración.
    /// </summary>
    public class ProductoConcreto : IProductoComponente
    {
        private readonly Producto _producto;

        /// <summary>
        /// Inicializa una nueva instancia del <see cref="ProductoConcreto"/>.
        /// </summary>
        /// <param name="producto">El objeto <see cref="Producto"/> de la base de datos que se va a representar como el componente base.</param>
        /// <exception cref="ArgumentNullException">Se lanza si el producto proporcionado es nulo.</exception>
        public ProductoConcreto(Producto producto)
        {
            _producto = producto ?? throw new ArgumentNullException(nameof(producto));
        }

        /// <summary>
        /// Obtiene el precio unitario base del producto, sin aplicar ningún decorador.
        /// (Implementación de <see cref="IProductoComponente.GetPrecio"/>).
        /// </summary>
        /// <returns>El precio base del producto.</returns>
        public decimal GetPrecio() => _producto.Precio;

        /// <summary>
        /// Obtiene la descripción base del producto (generalmente el nombre).
        /// (Implementación de <see cref="IProductoComponente.GetDescripcion"/>).
        /// </summary>
        /// <returns>El nombre del producto base.</returns>
        public string GetDescripcion() => _producto.Nombre;

        // Propiedad auxiliar para acceder al ID del producto si es necesario
        /// <summary>
        /// Obtiene el identificador único del producto base.
        /// </summary>
        public int IdProducto => _producto.IdProducto;
    }
}