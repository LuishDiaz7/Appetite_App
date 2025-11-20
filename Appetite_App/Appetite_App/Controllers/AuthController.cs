using Microsoft.AspNetCore.Mvc;
using Appetite_App.Services;
using Appetite_App.DTOs;
using Appetite_App.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Appetite_App.Controllers
{
    public class AuthController : Controller
    {
        // Variable inyectada: _userManagementService
        private readonly UserManagement _userManagementService;
        private readonly UserManager<Usuario> _userManager;

        // Constructor modificado para inyectar ambos servicios
        public AuthController(UserManagement userManagementService, UserManager<Usuario> userManager)
        {
            _userManagementService = userManagementService;
            _userManager = userManager;
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Verificación inicial de nulos (previene ArgumentNullException)
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Por favor, ingrese email y contraseña.");
                return View();
            }

            // CORRECCIÓN CRÍTICA: Se llama a _userManagementService y se espera Usuario? o null.
            // Esto resuelve los errores CS0019, CS0103 y CS8130.
            var user = await _userManagementService.Login(email, password);

            if (user != null)
            {
                // La contraseña es correcta; creamos Claims e iniciamos la sesión de Cookies.
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Nombre)
                };

                // Obtener roles del usuario desde Identity
                var roles = await _userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                // ----------------------------------------------------------------------------------
                // CORRECCIÓN 1: Usar IdentityConstants.ApplicationScheme para ClaimsIdentity
                // ----------------------------------------------------------------------------------
                var claimsIdentity = new ClaimsIdentity(
                    claims, IdentityConstants.ApplicationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                // ----------------------------------------------------------------------------------
                // CORRECCIÓN 2: Usar IdentityConstants.ApplicationScheme para SignInAsync
                // ----------------------------------------------------------------------------------
                await HttpContext.SignInAsync(
                    IdentityConstants.ApplicationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirigir según el rol
                if (await _userManager.IsInRoleAsync(user, "Administrador"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "Home");
            }

            // Si falla el login (user es null)
            ModelState.AddModelError("", "Credenciales inválidas.");
            return View();
        }

        // GET: /Auth/Registro
        public IActionResult Registro()
        {
            return View();
        }

        // POST: /Auth/Registro
        [HttpPost]
        public async Task<IActionResult> Registro(RegistroUsuarioDTO dto)
        {
            // Llama al servicio que usa el PATRÓN FACTORY METHOD
            Usuario? nuevoUser = await _userManagementService.RegistrarUsuario(dto);

            if (nuevoUser != null)
            {
                // Redirigir al usuario para que inicie sesión con sus nuevas credenciales
                return RedirectToAction("Login", "Auth");
            }

            ModelState.AddModelError("", "El email ya está registrado.");
            return View();
        }

        // POST: /Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Se usa el método Identity por defecto, que es más seguro.
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}