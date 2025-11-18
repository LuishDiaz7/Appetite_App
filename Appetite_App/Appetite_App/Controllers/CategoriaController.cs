using Microsoft.AspNetCore.Mvc;
using Appetite_App.Models;
using Appetite_App.Repositories;

namespace Appetite.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaController(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        private bool EsAdministrador()
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");
            return rol == "Administrador";
        }

        public async Task<IActionResult> Index() // Antes 'Categorias' en AdminController
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            var categorias = await _categoriaRepository.GetAllAsync();
            return View(categorias);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            try
            {
                await _categoriaRepository.AddAsync(categoria);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al crear categoría: " + ex.Message;
                return View(categoria);
            }
        }

        // Nota: Faltaba la acción de Editar en el código original, pero aquí se incluye
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            var categoria = await _categoriaRepository.GetByIdAsync(id);
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoria)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            try
            {
                await _categoriaRepository.UpdateAsync(categoria);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar categoría: " + ex.Message;
                return View(categoria);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            try
            {
                await _categoriaRepository.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar categoría: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}