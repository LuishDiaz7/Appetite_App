using Appetite_App.DTOs;
using Appetite_App.Models;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Factory
{
    /// <summary>
    /// Implementa el Creador Concreto (<c>Concrete Creator</c>) para la creación de objetos <see cref="Usuario"/>
    /// con el rol predeterminado de Cliente.
    /// Hereda de la clase base abstracta <see cref="UsuarioFactory"/>.
    /// 
    /// </summary>
    public class ClientFactory : UsuarioFactory
    {
        /// <summary>
        /// Implementa el método de fábrica (<c>Factory Method</c>) para crear un objeto <see cref="Usuario"/>
        /// a partir de los datos de registro de un nuevo Cliente.
        /// </summary>
        /// <remarks>
        /// Esta clase se limita a construir la instancia base de la entidad. La asignación final del rol 
        /// "Cliente" y el hash de la contraseña se realizan posteriormente en el servicio de gestión de usuarios 
        /// utilizando <c>UserManager</c> y <c>RoleManager</c> de Identity.
        /// </remarks>
        /// <param name="dto">El Objeto de Transferencia de Datos (<see cref="RegistroUsuarioDTO"/>) con la información del nuevo cliente.</param>
        /// <returns>Una nueva instancia de <see cref="Usuario"/> inicializada con los datos del DTO.</returns>
        public override Usuario CrearUsuario(RegistroUsuarioDTO dto)
        {
            // Construye el objeto base Usuario.
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
