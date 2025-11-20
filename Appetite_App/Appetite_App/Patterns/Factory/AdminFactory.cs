using Appetite_App.DTOs;
using Appetite_App.Models;

namespace Appetite_App.Patterns.Factory
{
    // Creador Concreto para Administrador
    public class AdminFactory : UsuarioFactory
    {
        public override Usuario CrearUsuario(RegistroUsuarioDTO dto)
        {
            // El Rol y la Contraseña (Hash) ya no son gestionados aquí.
            // Aquí solo creamos el objeto Usuario con sus propiedades básicas.
            return new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                UserName = dto.Email, // Identity usa UserName para el login, lo igualamos al Email
                PhoneNumber = dto.PhoneNumber,
            };
        }
    }
}
