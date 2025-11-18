using Appetite_App.Models;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Models
{
    public class PreOrden
    {
        [Key]
        public int IdOrden { get; set; }
        public int IdUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public string Direccion { get; set; } = string.Empty;
        public decimal Total { get; set; }

        // Propiedades de navegación
        public Usuario? Usuario { get; set; }
        public ICollection<DetalleOrden> Detalles { get; set; } = new List<DetalleOrden>();
    }
}
