using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Rol { get; set; } = "Cliente";

        // Propiedad de navegación
        public ICollection<PreOrden> Ordenes { get; set; } = new List<PreOrden>();
    }
}