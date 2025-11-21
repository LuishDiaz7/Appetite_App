using Microsoft.AspNetCore.Mvc;
using Appetite_App.DTOs;
using Appetite_App.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization; // Importante para [Authorize]
using System;

namespace Appetite_App.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        // 🚨 NUEVO: Objeto para manejar la autenticación (creación de cookies)
        private readonly SignInManager<Usuario> _signInManager;

        // 🚨 CAMBIO: Solo inyectamos los Identity Managers
        public AuthController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            // Si el usuario ya está autenticado, redirigir al home
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Por favor, ingrese email y contraseña.");
                return View();
            }

            // 1. Encontrar usuario por email
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                // 2. Usar SignInManager para verificar contraseña y emitir cookie
                var result = await _signInManager.PasswordSignInAsync(user, password,
                                                                    isPersistent: true,
                                                                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Opcional: Mantener la sesión para compatibilidad con código antiguo
                    // Usar Identity es preferible a la Session
                    if (await _userManager.IsInRoleAsync(user, "Administrador"))
                    {
                        HttpContext.Session.SetString("UsuarioRol", "Administrador");
                        return RedirectToAction("Index", "Admin");
                    }
                    HttpContext.Session.SetString("UsuarioRol", "Cliente");
                    return RedirectToAction("Index", "Home");
                }
            }

            // Si falla el login
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
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            // Crear el nuevo objeto Usuario
            var nuevoUser = new Usuario
            {
                UserName = dto.Email, // Usar Email como UserName para Identity
                Email = dto.Email,
                Nombre = dto.Nombre
            };

            // 1. Usar UserManager para crear el usuario y hashear la contraseña
            var result = await _userManager.CreateAsync(nuevoUser, dto.Password);

            if (result.Succeeded)
            {
                // 2. Asignar el rol "Cliente" por defecto
                await _userManager.AddToRoleAsync(nuevoUser, "Cliente");

                // Redirigir al login o iniciar sesión directamente
                return RedirectToAction("Login", "Auth");
            }

            // Si falla la creación (ej: email duplicado, requisitos de contraseña)
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }      
    }
}