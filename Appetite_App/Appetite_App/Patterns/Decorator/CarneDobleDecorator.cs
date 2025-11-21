namespace Appetite_App.Patterns.Decorator
{
    // CarneDoble en el diagrama
    public class CarneDobleDecorator : ProductoDecorator
    {
        private const decimal CostoExtra = 8000.00M;
        private const string DescripcionExtra = ", Carne Doble (+$8.000)";

        public CarneDobleDecorator(IProductoComponente componente) : base(componente) { }

        public override string GetDescripcion()
        {
            return _componente.GetDescripcion() + DescripcionExtra;
        }

        public override decimal GetPrecio()
        {
            return _componente.GetPrecio() + CostoExtra;
        }
    }
}
