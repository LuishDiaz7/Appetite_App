using Microsoft.AspNetCore.Mvc;
using Appetite_App.Models;
using Appetite_App.Repositories;
using BCrypt.Net;
using Appetite_App.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Appetite_App.Controllers
{
    /// <summary>
    /// Controlador encargado de la gestión de usuarios (CRUD) por parte de un administrador.
    /// Utiliza Identity Framework a través de <see cref="UserManager{TUser}"/> para consultar
    /// usuarios y sus roles, aunque algunas operaciones de persistencia usan el repositorio directo.
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UserManager<Usuario> _userManager;

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="UsuarioController"/>.
        /// </summary>
        /// <param name="usuarioRepository">El repositorio para operaciones CRUD directas en la base de datos de usuarios.</param>
        /// <param name="userManager">El gestor de usuarios proporcionado por ASP.NET Core Identity para el manejo de roles y hash de contraseñas.</param>
        public UsuarioController(IUsuarioRepository usuarioRepository, UserManager<Usuario> userManager)
        {
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
        }

        // =========================================================================================
        // GESTIÓN DE USUARIOS (Index)
        // =========================================================================================

        /// <summary>
        /// Muestra el listado de todos los usuarios registrados en el sistema,
        /// incluyendo sus roles asignados.
        /// </summary>
        /// <returns>Una vista que contiene una colección de objetos <see cref="UsuarioListViewModel"/>.</returns>
        public async Task<IActionResult> Index()
        {
            // 1. Obtener todos los usuarios del sistema Identity de forma síncrona para evitar problemas de contexto.
            List<Usuario> usuarios = _userManager.Users.ToList();

            // 2. Mapear cada Usuario a UsuarioListViewModel y obtener sus roles de forma asíncrona.
            List<UsuarioListViewModel> usuariosViewModel = new List<UsuarioListViewModel>();

            foreach (Usuario user in usuarios)
            {
                // Consulta los roles del usuario a través de Identity
                IList<string> roles = await _userManager.GetRolesAsync(user);

                usuariosViewModel.Add(new UsuarioListViewModel
                {
                    Id = user.Id,
                    Nombre = user.Nombre,
                    Email = user.Email,
                    Roles = roles.ToList() // Asignación de la lista de roles
                });
            }

            // 3. Enviar la lista de ViewModels a la vista de administración
            return View("~/Views/Admin/Usuarios.cshtml", usuariosViewModel);
        }

        // =========================================================================================
        // CREAR USUARIO
        // =========================================================================================

        /// <summary>
        /// Muestra el formulario para crear un nuevo usuario.
        /// </summary>
        /// <returns>La vista del formulario de creación de usuario.</returns>
        [HttpGet]
        public IActionResult Crear()
        {
            return View("~/Views/Admin/CrearUsuario.cshtml");
        }

        /// <summary>
        /// Procesa los datos enviados para crear un nuevo usuario.
        /// </summary>
        /// <param name="usuario">El objeto <see cref="Usuario"/> con los datos del formulario.</param>
        /// <returns>Redirecciona a la acción <see cref="Index"/> si es exitoso; de lo contrario, regresa a la vista de creación con el modelo.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            try
            {
                // NOTA: Para un sistema Identity completo, se usaría _userManager.CreateAsync
                // En este caso, se usa el repositorio directo.
                await _usuarioRepository.AddAsync(usuario);
                TempData["Success"] = "Usuario creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al crear usuario: " + ex.Message;
                return View("~/Views/Admin/CrearUsuario.cshtml", usuario);
            }
        }

        // =========================================================================================
        // EDITAR USUARIO
        // =========================================================================================

        /// <summary>
        /// Muestra el formulario para editar un usuario existente.
        /// </summary>
        /// <param name="id">El identificador único del usuario a editar.</param>
        /// <returns>La vista del formulario de edición con el usuario precargado o NotFound si no existe.</returns>
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            Usuario? usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            return View("~/Views/Admin/EditarUsuario.cshtml", usuario);
        }

        /// <summary>
        /// Procesa los datos enviados para actualizar un usuario existente, incluyendo la actualización del hash de la contraseña si se proporciona una nueva.
        /// </summary>
        /// <param name="usuario">El objeto <see cref="Usuario"/> con los datos actualizados.</param>
        /// <returns>Redirecciona a la acción <see cref="Index"/> si es exitoso; de lo contrario, regresa a la vista de edición con el modelo.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Usuario usuario)
        {
            try
            {
                // Lógica de actualización de hash de contraseña (mejor usar _userManager.UpdateAsync)
                if (string.IsNullOrEmpty(usuario.PasswordHash))
                {
                    // Si no se proporcionó una nueva contraseña, mantener el hash existente
                    Usuario? usuarioActual = await _usuarioRepository.GetByIdAsync(usuario.Id);
                    if (usuarioActual != null)
                    {
                        usuario.PasswordHash = usuarioActual.PasswordHash;
                    }
                }
                else
                {
                    // Hashear la nueva contraseña proporcionada
                    usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
                }

                await _usuarioRepository.UpdateAsync(usuario);
                TempData["Success"] = "Usuario actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
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

        /// <summary>
        /// Elimina un usuario del sistema.
        /// </summary>
        /// <param name="id">El identificador único del usuario a eliminar.</param>
        /// <returns>Redirecciona a la acción <see cref="Index"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                // NOTA: Para Identity, se usaría _userManager.DeleteAsync(user).
                await _usuarioRepository.DeleteAsync(id);
                TempData["Success"] = "Usuario eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar usuario: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
