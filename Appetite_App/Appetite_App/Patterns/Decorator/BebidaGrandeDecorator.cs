namespace Appetite_App.Patterns.Decorator
{
    // BebidaGrande en el diagrama
    public class BebidaGrandeDecorator : ProductoDecorator
    {
        private const decimal CostoExtra = 1.50M;
        private const string DescripcionExtra = ", Tamaño Grande (+$1.500)";

        public BebidaGrandeDecorator(IProductoComponente componente) : base(componente) { }

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
