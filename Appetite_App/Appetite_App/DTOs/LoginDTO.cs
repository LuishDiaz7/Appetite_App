using System.ComponentModel.DataAnnotations;

namespace Appetite_App.DTOs
{
    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) utilizado para la recepción de credenciales 
    /// de inicio de sesión de un usuario. Este DTO reemplaza el uso de parámetros sueltos (string email, string password)
    /// en la acción Login del controlador.
    /// </summary>
    public class LoginDTO
    {
        /// <summary>
        /// Obtiene o establece la dirección de correo electrónico del usuario.
        /// Este campo es requerido para el inicio de sesión.
        /// </summary>
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string Email { get; set; }

        /// <summary>
        /// Obtiene o establece la contraseña del usuario.
        /// Este campo es requerido para el inicio de sesión.
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si se debe recordar la sesión del usuario.
        /// </summary>
        public bool RememberMe { get; set; }
    }
}
