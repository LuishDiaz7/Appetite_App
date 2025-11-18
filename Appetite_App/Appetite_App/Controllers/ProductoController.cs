using Microsoft.AspNetCore.Mvc;
using Appetite_App.Models;
using Appetite_App.Repositories;

namespace Appetite.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public ProductoController(IProductoRepository productoRepository, ICategoriaRepository categoriaRepository)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
        }

        private bool EsAdministrador()
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");
            return rol == "Administrador";
        }

        public async Task<IActionResult> Index() // Antes 'Productos' en AdminController
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            var productos = await _productoRepository.GetAllAsync();
            return View(productos);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            ViewBag.Categorias = await _categoriaRepository.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Producto producto)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            try
            {
                await _productoRepository.AddAsync(producto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al crear producto: " + ex.Message;
                ViewBag.Categorias = await _categoriaRepository.GetAllAsync();
                return View(producto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            var producto = await _productoRepository.GetByIdAsync(id);
            ViewBag.Categorias = await _categoriaRepository.GetAllAsync();
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Producto producto)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            try
            {
                await _productoRepository.UpdateAsync(producto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar producto: " + ex.Message;
                ViewBag.Categorias = await _categoriaRepository.GetAllAsync();
                return View(producto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            try
            {
                await _productoRepository.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar producto: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
