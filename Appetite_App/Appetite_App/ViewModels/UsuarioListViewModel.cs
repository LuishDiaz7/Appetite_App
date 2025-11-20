using System.ComponentModel.DataAnnotations;

namespace Appetite_App.ViewModels
{
    // ViewModel específico para la lista de usuarios en la vista de administración
    public class UsuarioListViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        // Propiedad que antes estaba en el modelo, ahora la calculamos.
        [Display(Name = "Rol")]
        public string Rol { get; set; } = string.Empty;
    }
}
