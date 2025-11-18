using Appetite_App.Models;
using Appetite_App.Repositories;
using Appetite_App.DTOs;
using Appetite_App.Patterns.Factory;
// Usar System.Security.Cryptography para simular hashing
using System.Security.Cryptography;
using System.Text;

namespace Appetite_App.Services
{
    // UserManagement en el diagrama, que utiliza UserFactory
    public class UserManagement
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UserManagement(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // Método para simular un hash simple de contraseña
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Lógica de Login
        public async Task<Usuario?> Login(string email, string password)
        {
            string passwordHash = HashPassword(password);
            Usuario? user = await _usuarioRepository.GetByEmailAsync(email);

            if (user != null && user.PasswordHash == passwordHash)
            {
                return user;
            }
            return null;
        }

        // Lógica de Registro que utiliza el PATRÓN FACTORY METHOD
        public async Task<Usuario?> RegistrarUsuario(RegistroUsuarioDTO dto)
        {
            if (await _usuarioRepository.GetByEmailAsync(dto.Email) != null)
            {
                return null; // El usuario ya existe
            }

            // 1. Determinar qué Fábrica usar (PATRÓN FACTORY METHOD)
            UserFactory factory;
            if (dto.Rol == "Administrador")
            {
                factory = new AdminFactory();
            }
            else // Asume Cliente por defecto
            {
                factory = new ClientFactory();
            }

            string passwordHash = HashPassword(dto.Password);

            // 2. Crear el objeto Usuario usando la Fábrica
            Usuario nuevoUsuario = factory.CrearUsuario(dto.Nombre, dto.Email, passwordHash);

            // 3. Persistir en el repositorio
            await _usuarioRepository.AddAsync(nuevoUsuario);
            return nuevoUsuario;
        }
        // Nuevo método para que el administrador pueda ver todos los usuarios
        public async Task<List<Usuario>> ObtenerTodos()
        {
            return (await _usuarioRepository.GetAllAsync()).ToList();
        }
    }
}
