namespace Appetite_App.DTOs
{
    public class RegistroUsuarioDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
        public string Rol { get; set; } = "Cliente"; // Rol por defecto
    }
}
