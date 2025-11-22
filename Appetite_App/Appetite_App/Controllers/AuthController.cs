using Microsoft.AspNetCore.Mvc;
using Appetite_App.DTOs;
using Appetite_App.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Appetite_App.Controllers
{
    /// <summary>
    /// Maneja la autenticación y el registro de usuarios utilizando ASP.NET Core Identity.
    /// </summary>
    public class AuthController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="AuthController"/>.
        /// </summary>
        /// <param name="userManager">El gestor de usuarios de ASP.NET Core Identity para operaciones CRUD de usuarios.</param>
        /// <param name="signInManager">El gestor de inicio de sesión de ASP.NET Core Identity para la autenticación y emisión de cookies.</param>
        public AuthController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ---------------------------------------------
        // INICIO DE SESIÓN (LOGIN)
        // ---------------------------------------------

        /// <summary>
        /// Muestra la página del formulario de inicio de sesión.
        /// Redirecciona al usuario al Home si ya está autenticado.
        /// </summary>
        /// <returns>La vista del formulario de inicio de sesión o una redirección al Home.</returns>
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        /// <summary>
        /// Procesa el intento de inicio de sesión del usuario.
        /// </summary>
        /// <param name="model">Las credenciales del usuario (email y contraseña).</param>
        /// <returns>
        /// Redirecciona al panel de administrador si el rol es "Administrador", 
        /// al Home si es "Cliente" y el login es exitoso.
        /// Retorna la vista con errores de validación si las credenciales son inválidas o faltan.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Buena práctica de seguridad
        public async Task<IActionResult> Login(LoginDTO model) // Usamos LoginDTO para estandarizar el input
        {
            // Validar si el modelo es nulo o si las propiedades requeridas son nulas/vacías.
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError(string.Empty, "Por favor, ingrese email y contraseña.");
                return View(model);
            }

            // 1. Encontrar usuario por email
            Usuario user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                // 2. Usar SignInManager para verificar contraseña y emitir cookie.
                // isPersistent: true = "Remember Me"
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    isPersistent: true, // Asumimos que la persistencia se maneja por defecto en este punto o se añade al DTO si es necesario
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Asignar el rol a la sesión (opcional, ya que Identity lo maneja con Claims)
                    if (await _userManager.IsInRoleAsync(user, "Administrador"))
                    {
                        HttpContext.Session.SetString("UsuarioRol", "Administrador");
                        return RedirectToAction("Index", "Admin");
                    }
                    HttpContext.Session.SetString("UsuarioRol", "Cliente");
                    return RedirectToAction("Index", "Home");
                }
            }

            // Si falla el login o el usuario es nulo
            ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
            return View(model);
        }

        // ---------------------------------------------
        // REGISTRO
        // ---------------------------------------------

        /// <summary>
        /// Muestra la página del formulario de registro de un nuevo usuario.
        /// </summary>
        /// <returns>La vista del formulario de registro.</returns>
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        /// <summary>
        /// Procesa el registro de un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="dto">Los datos del nuevo usuario para el registro.</param>
        /// <returns>
        /// Redirecciona a la acción <see cref="Login"/> si el registro es exitoso; 
        /// de lo contrario, regresa a la vista de <see cref="Registro"/> con errores de validación.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroUsuarioDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            // Crear el nuevo objeto Usuario
            Usuario nuevoUser = new Usuario
            {
                UserName = dto.Email, // Usar Email como UserName para Identity
                Email = dto.Email,
                Nombre = dto.Nombre
            };

            // 1. Usar UserManager para crear el usuario y hashear la contraseña
            IdentityResult result = await _userManager.CreateAsync(nuevoUser, dto.Password);

            if (result.Succeeded)
            {
                // 2. Asignar el rol "Cliente" por defecto
                await _userManager.AddToRoleAsync(nuevoUser, "Cliente");

                // Redirigir al login
                TempData["SuccessMessage"] = "Registro exitoso. Por favor, inicie sesión.";
                return RedirectToAction("Login", "Auth");
            }

            // Si falla la creación (ej: email duplicado, requisitos de contraseña)
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(dto);
        }

        // ---------------------------------------------
        // CIERRE DE SESIÓN (LOGOUT)
        // ---------------------------------------------

        /// <summary>
        /// Realiza el cierre de sesión del usuario actual y limpia la sesión.
        /// </summary>
        /// <returns>Redirecciona a la acción <see cref="HomeController.Index"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            // Limpiar la sesión HTTP (incluyendo el rol almacenado opcionalmente)
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}