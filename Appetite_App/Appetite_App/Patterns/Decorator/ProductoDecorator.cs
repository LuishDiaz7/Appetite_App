namespace Appetite_App.Patterns.Decorator
{
    // Agrega en el diagrama
    public abstract class ProductoDecorator : IProductoComponente
    {
        protected IProductoComponente _componente;

        public ProductoDecorator(IProductoComponente componente)
        {
            _componente = componente;
        }

        // El decorador debe implementar la interfaz para delegar o agregar funcionalidad
        public virtual string GetDescripcion()
        {
            return _componente.GetDescripcion();
        }

        public virtual decimal GetPrecio()
        {
            return _componente.GetPrecio();
        }
    }
}
