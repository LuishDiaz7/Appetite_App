using Appetite_App.DTOs;
using Appetite_App.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Factory
{
    /// <summary>
    /// Implementa el Creador Concreto (<c>Concrete Creator</c>) para la creación de objetos <see cref="Usuario"/>
    /// con un enfoque específico para el rol de Administrador.
    /// Hereda de la clase base abstracta <see cref="UsuarioFactory"/>.
    /// 
    /// </summary>
    public class AdminFactory : UsuarioFactory
    {
        /// <summary>
        /// Implementa el método de fábrica (<c>Factory Method</c>) para crear un objeto <see cref="Usuario"/>
        /// a partir de los datos de registro.
        /// </summary>
        /// <remarks>
        /// La asignación final del rol ('Administrador') y el hash de la contraseña se gestionan
        /// posteriormente en el <see cref="Services.UserManagement"/> usando el <c>UserManager</c> de Identity.
        /// Esta clase se limita a construir la instancia base de la entidad.
        /// </remarks>
        /// <param name="dto">El Objeto de Transferencia de Datos (<see cref="RegistroUsuarioDTO"/>) con la información del nuevo usuario.</param>
        /// <returns>Una nueva instancia de <see cref="Usuario"/> inicializada con los datos del DTO.</returns>
        public override Usuario CrearUsuario(RegistroUsuarioDTO dto)
        {
            return new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                UserName = dto.Email, // Identity usa UserName para el login, lo igualamos al Email
                PhoneNumber = dto.PhoneNumber,
                // Nota: La contraseña se establece fuera de la Factory por Identity
            };
        }
    }
}