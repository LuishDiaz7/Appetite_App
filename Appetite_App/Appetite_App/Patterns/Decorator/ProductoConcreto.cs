namespace Appetite_App.Patterns.Decorator
{
    using Appetite_App.Models; // Asume que tu modelo Producto está aquí

    public class ProductoConcreto : IProductoComponente
    {
        private readonly Producto _producto;

        // Constructor que recibe el objeto de la base de datos
        public ProductoConcreto(Producto producto)
        {
            _producto = producto ?? throw new ArgumentNullException(nameof(producto));
        }

        // Implementa la interfaz para devolver el precio base del producto
        public decimal GetPrecio() => _producto.Precio;

        // Implementa la interfaz para devolver el nombre/descripción base
        public string GetDescripcion() => _producto.Nombre;
    }
}