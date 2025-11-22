using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq; // Necesario para .Any() y .First()

namespace Appetite_App.ViewModels
{
    /// <summary>
    /// Modelo de Vista (<c>ViewModel</c>) utilizado para mostrar una lista concisa de usuarios
    /// en la interfaz de administración. Contiene solo los datos necesarios para la presentación tabular.
    /// </summary>
    public class UsuarioListViewModel
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre completo del usuario.
        /// </summary>
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del usuario, utilizado como nombre de inicio de sesión.
        /// </summary>
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Lista de todos los roles de Identity asociados a este usuario (ej. "Cliente", "Administrador").
        /// </summary>
        [Display(Name = "Roles")]
        public List<string> Roles { get; set; } = new List<string>();

        /// <summary>
        /// Propiedad calculada que devuelve el primer rol de la lista. 
        /// Útil para mostrar un solo identificador de rol en una tabla.
        /// </summary>
        /// <value>El primer rol si existe; de lo contrario, "N/A".</value>
        public string RolPrincipal => Roles != null && Roles.Any() ? Roles.First() : "N/A";
    }
}