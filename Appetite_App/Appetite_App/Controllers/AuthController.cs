using Microsoft.AspNetCore.Mvc;
using Appetite_App.Services;
using Appetite_App.DTOs;
using Appetite_App.Models;
// Usamos System.Security.Claims para la simulación de login
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Appetite_App.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManagement _userManager;

        public AuthController(UserManagement userManager)
        {
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
            // Nota: El servicio UserManagement (que usa Factory) NO se usa directamente en el login, 
            // pero sí usa el repositorio que él orquesta.
            Usuario? user = await _userManager.Login(email, password);

            if (user != null)
            {
                // **Simulación de Login (Autenticación por Cookies)**
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Rol), // Rol para autorizar
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                if (user.Rol == "Administrador")
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "Cliente");
            }

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
            Usuario? nuevoUser = await _userManager.RegistrarUsuario(dto);

            if (nuevoUser != null)
            {
                // Iniciar sesión inmediatamente
                await Login(dto.Email, dto.Password);
                return RedirectToAction("Index", "Cliente");
            }

            ModelState.AddModelError("", "El email ya está registrado.");
            return View();
        }

        // POST: /Auth/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
