using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Models
{
    /// <summary>
    /// Extiende la clase base de Identity (<c>IdentityUser</c>) para incluir propiedades
    /// específicas de la aplicación, como el nombre del usuario, mientras mantiene
    /// las funcionalidades clave de autenticación y autorización de ASP.NET Core Identity.
    /// <c>IdentityUser&lt;int&gt;</c> indica que la clave primaria (<c>Id</c>) es de tipo entero.
    /// </summary>
    public class Usuario : IdentityUser<int>
    {
        // Nota: Las propiedades como Email, PasswordHash, PhoneNumber, etc., son heredadas de IdentityUser<int>.

        /// <summary>
        /// Obtiene o establece el nombre completo o el alias del usuario.
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        // Propiedad de navegación

        /// <summary>
        /// Obtiene o establece la colección de órdenes (<see cref="PreOrden"/>) realizadas por este usuario.
        /// (Relación uno a muchos: un Usuario tiene muchas PreOrdenes).
        /// </summary>
        public ICollection<PreOrden> Ordenes { get; set; } = new List<PreOrden>();
    }
}