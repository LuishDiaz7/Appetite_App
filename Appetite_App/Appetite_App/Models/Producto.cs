using Appetite_App.Models;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public bool Activo { get; set; } = true;

        // Clave Foránea
        public int IdCategoria { get; set; }

        // Propiedad de navegación
        public Categoria? Categoria { get; set; }

        // Propiedad de Navegación: Colección de detalles de órdenes donde aparece este producto
        public ICollection<DetalleOrden> DetallesOrden { get; set; } = new List<DetalleOrden>();
    }
}