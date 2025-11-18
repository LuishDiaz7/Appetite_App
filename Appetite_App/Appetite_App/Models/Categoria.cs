using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Propiedad de navegación
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
