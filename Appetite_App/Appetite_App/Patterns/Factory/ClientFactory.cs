using Appetite_App.Models;

namespace Appetite_App.Patterns.Factory
{
    // ClientFactory en el diagrama
    public class ClientFactory : UserFactory
    {
        public override Usuario CrearUsuario(string nombre, string email, string passwordHash)
        {
            return new Usuario
            {
                Nombre = nombre,
                Email = email,
                PasswordHash = passwordHash,
                Rol = "Cliente" // ROL CLAVE
            };
        }
    }
}
