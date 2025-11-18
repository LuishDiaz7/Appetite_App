using Appetite_App.Models;

namespace Appetite_App.Patterns.Factory
{
    // Clase abstracta base que usa el Patrón Factory Method
    public abstract class UserFactory
    {
        // El Factory Method que las subclases deben implementar
        public abstract Usuario CrearUsuario(string nombre, string email, string passwordHash);
    }
}
