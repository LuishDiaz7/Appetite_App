namespace Appetite_App.Patterns.Decorator
{
    // QuesoExtra en el diagrama
    public class QuesoExtraDecorator : ProductoDecorator
    {
        private const decimal CostoExtra = 4000.00M;
        private const string DescripcionExtra = ", Queso Extra (+$4.000)";

        public QuesoExtraDecorator(IProductoComponente componente) : base(componente) { }

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
