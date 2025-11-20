using Appetite_App.DTOs;
using Appetite_App.Models;

namespace Appetite_App.Patterns.Factory
{
    // Creador Concreto para Cliente
    public class ClientFactory : UsuarioFactory
    {
        public override Usuario CrearUsuario(RegistroUsuarioDTO dto)
        {
            // El Rol ya no es una propiedad simple, se gestiona con Identity.
            // Aquí solo creamos el objeto Usuario con sus propiedades básicas.
            return new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                UserName = dto.Email, // Identity usa UserName para el login
                PhoneNumber = dto.PhoneNumber,
            };
        }
    }
}
