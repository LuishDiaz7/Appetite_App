using System.ComponentModel.DataAnnotations;

namespace Appetite_App.ViewModels
{
    /// <summary>
    /// Modelo de Vista (<c>ViewModel</c>) utilizado para transferir y mostrar
    /// información esencial de un usuario individual (ej. en una página de perfil o resumen).
    /// </summary>
    public class UsuarioViewModel
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre completo del usuario.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Rol principal asignado al usuario (ej. "Cliente", "Administrador").
        /// </summary>
        public string Rol { get; set; } = string.Empty;
    }
}