using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.DTOs
{
    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) utilizado para el registro de nuevos usuarios en el sistema.
    /// Contiene los campos necesarios para la creación de una cuenta, incluyendo la selección de rol inicial.
    /// </summary>
    public class RegistroUsuarioDTO
    {
        /// <summary>
        /// Obtiene o establece el nombre completo o alias del usuario.
        /// Este campo es requerido.
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece la dirección de correo electrónico del usuario.
        /// Este campo es requerido y debe ser un formato de email válido.
        /// </summary>
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string Email { get; set; }

        /// <summary>
        /// Obtiene o establece el número de teléfono del usuario.
        /// Este campo es opcional pero se utiliza para la creación de la entidad base en la Factory.
        /// </summary>
        [Phone(ErrorMessage = "Formato de teléfono inválido.")]
        [DisplayName("Número de Teléfono")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Obtiene o establece la contraseña para la nueva cuenta.
        /// Este campo es requerido y debe tener al menos 6 caracteres.
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Obtiene o establece la confirmación de la contraseña.
        /// Debe coincidir con la propiedad <see cref="Password"/>.
        /// </summary>
        [DataType(DataType.Password)]
        [DisplayName("Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Obtiene o establece el rol del usuario ('Administrador' o 'Cliente').
        /// Este campo es crucial para la lógica del Factory Method.
        /// </summary>
        [Required(ErrorMessage = "El Rol es obligatorio.")]
        public string Rol { get; set; }
    }
}