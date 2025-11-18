using Appetite_App.Models;

namespace Appetite_App.Models
{
    public class DetalleOrden
    {
        public int IdDetalle { get; set; }
        public int IdOrden { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        // Campo para almacenar los decoradores aplicados (ej: "Queso Extra, Carne Doble")
        public string DecoradoresAplicados { get; set; } = string.Empty;

        // Propiedades de navegación
        public PreOrden? Orden { get; set; }
        public Producto? Producto { get; set; }

        public PreOrden PreOrden { get; set; } = default!;
    }
}
