using Microsoft.AspNetCore.Mvc;
using Appetite_App.Models;
using Appetite_App.Repositories;
using BCrypt.Net;
using Appetite_App.ViewModels; // Necesario para UsuarioListViewModel
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Appetite.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UserManager<Usuario> _userManager;

        public UsuarioController(IUsuarioRepository usuarioRepository, UserManager<Usuario> userManager)
        {
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
        }

        // =========================================================================================
        // GESTIÓN DE USUARIOS (Index)
        // =========================================================================================

        public async Task<IActionResult> Index()
        {
            // 1. Obtener todos los usuarios del sistema Identity
            // 🚨 CORRECCIÓN THREADING: Usamos ToList() sincrónico para forzar la ejecución
            // inmediata de la consulta y evitar el error de DbContext en uso.
            var usuarios = _userManager.Users.ToList();

            // 2. Mapear cada Usuario a UsuarioListViewModel y obtener sus roles
            var usuariosViewModel = new List<UsuarioListViewModel>();

            foreach (var user in usuarios)
            {
                // Esta es una operación asíncrona (consulta a la BD)
                var roles = await _userManager.GetRolesAsync(user);

                // 🚨 CORRECCIÓN CS0117: Asignamos a la propiedad 'Roles' (plural)
                usuariosViewModel.Add(new UsuarioListViewModel
                {
                    Id = user.Id,
                    Nombre = user.Nombre,
                    Email = user.Email,
                    Roles = roles.ToList() // Asignación correcta
                });
            }

            // 3. Enviar la lista de ViewModels a la vista
            // 🚨 CORRECCIÓN VISTAS: Apuntar explícitamente a la ubicación de la vista 'Usuarios'
            // ya que está dentro de la carpeta Admin.
            return View("~/Views/Admin/Usuarios.cshtml", usuariosViewModel);
        }

        // =========================================================================================
        // CREAR USUARIO
        // =========================================================================================

        [HttpGet]
        public IActionResult Crear()
        {
            return View("~/Views/Admin/CrearUsuario.cshtml"); // 🚨 CORRECCIÓN VISTAS
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            try
            {
                // NOTA: Se recomienda refactorizar para usar _userManager.CreateAsync
                // para que Identity gestione el hashing y la asignación de IDs.
                await _usuarioRepository.AddAsync(usuario);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al crear usuario: " + ex.Message;
                return View("~/Views/Admin/CrearUsuario.cshtml", usuario); // 🚨 CORRECCIÓN VISTAS
            }
        }

        // =========================================================================================
        // EDITAR USUARIO
        // =========================================================================================

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            return View("~/Views/Admin/EditarUsuario.cshtml", usuario); // Suponiendo que existe esta vista
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Usuario usuario)
        {
            try
            {
                // Lógica de actualización de hash (temporal, mejor usar _userManager.UpdateAsync)
                if (string.IsNullOrEmpty(usuario.PasswordHash))
                {
                    var usuarioActual = await _usuarioRepository.GetByIdAsync(usuario.Id);
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
                return View("~/Views/Admin/EditarUsuario.cshtml", usuario);
            }
        }

        // =========================================================================================
        // ELIMINAR USUARIO
        // =========================================================================================

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                // NOTA: Se recomienda refactorizar para usar _userManager.DeleteAsync
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
