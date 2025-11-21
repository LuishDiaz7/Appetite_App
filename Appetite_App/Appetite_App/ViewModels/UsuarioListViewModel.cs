using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; 

namespace Appetite_App.ViewModels
{
    public class UsuarioListViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Roles")] 
        public List<string> Roles { get; set; } = new List<string>();

        public string RolPrincipal => Roles != null && Roles.Any() ? Roles.First() : "N/A";
    }
}
