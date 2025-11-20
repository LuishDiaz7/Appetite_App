using Appetite_App.DTOs;
using Appetite_App.Models;
using Appetite_App.Patterns.Factory;
using Appetite_App.Repositories; // Aún necesario si se usa para otras operaciones no Auth
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic; // Necesario para List/IEnumerable
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Appetite_App.Services
{
    // Este servicio orquesta la creación y autenticación de usuarios usando Identity
    public class UserManagement
    {
        // Servicios de Identity necesarios para la gestión de usuarios
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        // Mantener el repositorio si se usa para otras consultas CRUD específicas
        private readonly IUsuarioRepository _usuarioRepository;

        public UserManagement(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            IUsuarioRepository usuarioRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _usuarioRepository = usuarioRepository;
        }

        // CORRECCIÓN 1: Lógica de Login usando Identity
        public async Task<Usuario?> Login(string email, string password)
        {
            // Verificación para evitar ArgumentNullException (ya debe tener este bloque)
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null; // Cambiado de (false, null) a null
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null; // Cambiado de (false, null) a null
            }

            // Usamos CheckPasswordSignInAsync, que solo verifica la contraseña.
            // Usar PasswordSignInAsync puede interferir con la lógica de cookies del controlador.
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return user; // Cambiado de (true, user) a user
            }

            return null; // Cambiado de (false, null) a null
        }

        // CORRECCIÓN 2: Lógica de Registro usando Identity y Patrón Factory
        public async Task<Usuario?> RegistrarUsuario(RegistroUsuarioDTO dto)
        {
            // Usamos UserManager para verificar si el usuario ya existe por email
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
            {
                return null; // El usuario ya existe
            }

            // 1. Determinar qué Fábrica usar y el rol a asignar (PATRÓN FACTORY METHOD)
            UsuarioFactory factory;
            string rolAsignado;

            if (dto.Rol == "Administrador")
            {
                factory = new AdminFactory();
                rolAsignado = "Administrador";
            }
            else // Asume Cliente por defecto
            {
                factory = new ClientFactory();
                rolAsignado = "Cliente";
            }

            // 2. Crear el objeto Usuario usando la Fábrica
            var user = factory.CrearUsuario(dto);
            // Nota: El método CrearUsuario en la Factory ya no debe recibir el hash de password.

            // 3. Persistir y hashear la contraseña usando Identity UserManager
            // Identity usa el campo 'UserName' para el login, lo configuramos para usar el Email
            user.UserName = dto.Email;

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                // 4. Asignar el rol usando Identity Role Manager
                if (!await _roleManager.RoleExistsAsync(rolAsignado))
                {
                    // Esto se repite aquí por si acaso, aunque idealmente ya existe desde DbInitializer
                    await _roleManager.CreateAsync(new IdentityRole<int>(rolAsignado));
                }

                await _userManager.AddToRoleAsync(user, rolAsignado);

                return user;
            }

            return null; // Falló la creación
        }

        // Nuevo método para que el administrador pueda ver todos los usuarios (usa el repositorio o UserManager)
        public async Task<List<Usuario>> ObtenerTodos()
        {
            // Es mejor usar UserManager para obtener la lista, ya que está diseñado para ello.
            return (await _userManager.Users.ToListAsync());
        }
    }
}

