using Appetite_App.Services;
using Microsoft.AspNetCore.Mvc;
using Appetite_App.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;
using Appetite_App.Models; // Necesario para referenciar el modelo Categoria

namespace Appetite_App.Controllers
{
    /// <summary>
    /// Controlador principal que maneja la página de inicio de la aplicación,
    /// mostrando el catálogo de productos organizado por categorías.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaRepository _categoriaRepository;

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="HomeController"/>.
        /// </summary>
        /// <param name="productoService">El servicio para acceder a la lógica de negocio de productos (aunque no se usa directamente en Index, se inyecta por si otros métodos lo necesitan).</param>
        /// <param name="categoriaRepository">El repositorio para acceder a la capa de persistencia de categorías.</param>
        public HomeController(IProductoService productoService, ICategoriaRepository categoriaRepository)
        {
            _productoService = productoService;
            _categoriaRepository = categoriaRepository;
        }

        /// <summary>
        /// Muestra la página de inicio de la aplicación, cargando todas las categorías
        /// junto con sus productos asociados.
        /// </summary>
        /// <returns>Una vista que contiene una colección de objetos <see cref="Categoria"/>.</returns>
        public async Task<IActionResult> Index()
        {
            // Nota: Se asume que ICategoriaRepository tiene el método GetAllWithProductsAsync
            IEnumerable<Categoria> categorias = await _categoriaRepository.GetAllWithProductsAsync();

            return View(categorias);
        }

        // ... Otros métodos del controlador (e.g., Privacy, Error) pueden ir aquí, documentados de igual forma ...

        // Ejemplo de método adicional:
        /// <summary>
        /// Muestra la página de privacidad de la aplicación.
        /// </summary>
        /// <returns>La vista de privacidad.</returns>
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
