using Appetite_App.Models;

namespace Appetite_App.Patterns.Factory
{
    // AdminFactory en el diagrama
    public class AdminFactory : UserFactory
    {
        public override Usuario CrearUsuario(string nombre, string email, string passwordHash)
        {
            return new Usuario
            {
                Nombre = nombre,
                Email = email,
                PasswordHash = passwordHash,
                Rol = "Administrador" // ROL CLAVE
            };
        }
    }
}
