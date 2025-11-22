using Appetite_App.DTOs;
using Appetite_App.Models;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Patterns.Factory
{
    /// <summary>
    /// Clase abstracta que actúa como el Creador (<c>Creator</c>) en el Patrón Factory Method.
    /// Define el método de fábrica abstracto (<c>Factory Method</c>) que las subclases deben implementar
    /// para crear instancias del Producto (<see cref="Usuario"/>).
    /// 
    /// </summary>
    public abstract class UsuarioFactory
    {
        /// <summary>
        /// Método de fábrica abstracto que las subclases concretas implementarán para crear un objeto <see cref="Usuario"/>.
        /// Este método delega la responsabilidad de la instanciación a las subclases (<see cref="AdminFactory"/> o <see cref="ClientFactory"/>).
        /// </summary>
        /// <param name="dto">El Objeto de Transferencia de Datos (<see cref="RegistroUsuarioDTO"/>) con la información base del usuario.</param>
        /// <returns>Un objeto de tipo <see cref="Usuario"/>.</returns>
        public abstract Usuario CrearUsuario(RegistroUsuarioDTO dto);
    }
}
