using Microsoft.AspNetCore.Mvc;
using Appetite_App.Models;
using Appetite_App.Repositories;
using BCrypt.Net; 

namespace Appetite.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        private bool EsAdministrador()
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");
            return rol == "Administrador";
        }

        public async Task<IActionResult> Index() // Antes 'Usuarios' en AdminController
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            var usuarios = await _usuarioRepository.GetAllAsync();
            return View(usuarios);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            try
            {
                // La lógica de hashing debe estar en la capa de servicio/repositorio, pero se deja aquí para la migración
                // Si la contraseña no viene hasheada, asume que debe hashearse aquí
                // usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash); 

                await _usuarioRepository.AddAsync(usuario);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al crear usuario: " + ex.Message;
                return View(usuario);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Usuario usuario)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            try
            {
                if (string.IsNullOrEmpty(usuario.PasswordHash))
                {
                    var usuarioActual = await _usuarioRepository.GetByIdAsync(usuario.IdUsuario);
                    usuario.PasswordHash = usuarioActual.PasswordHash;
                }
                else
                {
                    usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
                }

                await _usuarioRepository.UpdateAsync(usuario);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar usuario: " + ex.Message;
                return View(usuario);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            try
            {
                await _usuarioRepository.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar usuario: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}