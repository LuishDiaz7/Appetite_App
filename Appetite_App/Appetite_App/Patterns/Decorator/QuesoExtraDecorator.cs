namespace Appetite_App.Patterns.Decorator
{
    // QuesoExtra en el diagrama
    public class QuesoExtraDecorator : ProductoDecorator
    {
        private const decimal CostoExtra = 2.00M;
        private const string DescripcionExtra = ", Queso Extra (+$2.000)";

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
