using Appetite_App.Models; 

namespace Appetite_App.Patterns.Decorator
{
    // ProductoConcreto en el diagrama
    public class ProductoConcreto : IProductoComponente
    {
        private readonly Producto _producto;

        public ProductoConcreto(Producto producto)
        {
            _producto = producto;
        }

        public string GetDescripcion()
        {
            return _producto.Nombre;
        }

        public decimal GetPrecio()
        {
            return _producto.Precio;
        }
    }
}
