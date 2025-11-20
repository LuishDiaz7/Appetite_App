using Appetite_App.DTOs;
using Appetite_App.Models;

namespace Appetite_App.Patterns.Factory
{
    public abstract class UsuarioFactory
    {
        public abstract Usuario CrearUsuario(RegistroUsuarioDTO dto);
    }
}
