using Appetite_App.DTOs;
using Appetite_App.Models;
using Appetite_App.Patterns.Factory;
using Appetite_App.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace Appetite_App.Services
{
    /// <summary>
    /// Servicio de gestión de usuarios que integra las funcionalidades de ASP.NET Core Identity
    /// (<c>UserManager</c>, <c>SignInManager</c>, <c>RoleManager</c>) y el Patrón Factory Method
    /// para manejar la autenticación, el registro y la creación de diferentes tipos de usuarios
    /// (Administrador o Cliente).
    /// </summary>
    public class UserManagement
    {
        // Servicios de Identity necesarios para la gestión de usuarios
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        // Repositorio para consultas CRUD específicas que complementen a Identity
        private readonly IUsuarioRepository _usuarioRepository;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UserManagement"/> con todas sus dependencias.
        /// </summary>
        /// <param name="userManager">Servicio para gestionar usuarios.</param>
        /// <param name="signInManager">Servicio para manejar el inicio de sesión y la validación de credenciales.</param>
        /// <param name="roleManager">Servicio para gestionar los roles de Identity.</param>
        /// <param name="usuarioRepository">Repositorio de acceso a datos de la entidad <see cref="Usuario"/>.</param>
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

        /// <summary>
        /// Intenta iniciar sesión validando las credenciales del usuario contra la base de datos de Identity.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario.</param>
        /// <param name="password">La contraseña proporcionada.</param>
        /// <returns>El objeto <see cref="Usuario"/> si la autenticación es exitosa; de lo contrario, <c>null</c>.</returns>
        public async Task<Usuario?> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            // Usamos CheckPasswordSignInAsync para verificar solo la contraseña y evitar efectos secundarios 
            // de autenticación de cookies que deben manejarse en el controlador.
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return user;
            }

            return null;
        }

        /// <summary>
        /// Registra un nuevo usuario en la aplicación utilizando el Patrón Factory Method.
        /// 
        /// </summary>
        /// <param name="dto">El DTO (<see cref="RegistroUsuarioDTO"/>) que contiene la información de registro, incluyendo el rol deseado.</param>
        /// <returns>El objeto <see cref="Usuario"/> recién creado y persistido con su rol asignado, o <c>null</c> si el registro falló (ej. usuario ya existe).</returns>
        public async Task<Usuario?> RegistrarUsuario(RegistroUsuarioDTO dto)
        {
            // 1. Verificación: El usuario ya existe por email
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
            {
                return null; // El usuario ya existe
            }

            // 2. Determinar qué Fábrica usar y el rol a asignar (PATRÓN FACTORY METHOD)
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

            // 3. Crear el objeto Usuario usando la Fábrica
            var user = factory.CrearUsuario(dto);
            user.UserName = dto.Email; // Identity usa el campo 'UserName' para el login

            // 4. Persistir y hashear la contraseña usando Identity UserManager
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                // 5. Asignar el rol usando Identity Role Manager
                if (!await _roleManager.RoleExistsAsync(rolAsignado))
                {
                    // Bloque de seguridad: crea el rol si por alguna razón no lo hizo DbInitializer
                    await _roleManager.CreateAsync(new IdentityRole<int>(rolAsignado));
                }

                await _userManager.AddToRoleAsync(user, rolAsignado);

                return user;
            }

            // Falló la creación
            return null;
        }

        /// <summary>
        /// Obtiene una lista de todos los usuarios registrados en el sistema de manera asíncrona.
        /// </summary>
        /// <remarks>
        /// Este método está destinado a ser utilizado por administradores para propósitos de gestión.
        /// </remarks>
        /// <returns>Una lista de todos los objetos <see cref="Usuario"/>.</returns>
        public async Task<List<Usuario>> ObtenerTodos()
        {
            // Se utiliza el UserManager para obtener la lista, aprovechando su integración con EF Core.
            return (await _userManager.Users.ToListAsync());
        }
    }
}

