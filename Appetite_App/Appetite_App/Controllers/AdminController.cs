using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Appetite_App.Repositories;
using Appetite_App.Models;
using Appetite_App.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Appetite_App.Controllers
{
    /// <summary>
    /// Proporciona funcionalidades exclusivas para usuarios con el rol de Administrador.
    /// Este controlador se enfoca principalmente en la visualización y gestión del estado de las órdenes.
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly IOrdenRepository _ordenRepository;
        private readonly OrdenService _ordenService;

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="AdminController"/>.
        /// </summary>
        /// <param name="ordenRepository">El repositorio para acceder a la capa de persistencia de las órdenes.</param>
        /// <param name="ordenService">El servicio que contiene la lógica de negocio para la gestión de órdenes (incluyendo el patrón Observer).</param>
        public AdminController(
            IOrdenRepository ordenRepository,
            OrdenService ordenService)
        {
            _ordenRepository = ordenRepository;
            _ordenService = ordenService;
        }

        /// <summary>
        /// Muestra la página de inicio o panel principal del administrador.
        /// </summary>
        /// <returns>La vista del panel de administración.</returns>
        public IActionResult Index()
        {
            return View();
        }

        // ---------------------------------------------
        // GESTIÓN DE ÓRDENES
        // ---------------------------------------------

        /// <summary>
        /// Muestra la lista de todas las órdenes registradas en el sistema.
        /// </summary>
        /// <returns>Una vista que contiene una colección de objetos <see cref="PreOrden"/>.</returns>
        public async Task<IActionResult> Ordenes()
        {
            // Asume que GetAllAsync retorna IEnumerable<PreOrden>
            IEnumerable<PreOrden> ordenes = await _ordenRepository.GetAllAsync();
            return View(ordenes);
        }

        /// <summary>
        /// Cambia el estado de una orden específica y notifica a los observadores (Inventario, Notificación, Auditoría).
        /// </summary>
        /// <param name="idOrden">El identificador único de la orden cuyo estado se desea modificar.</param>
        /// <param name="estado">El nuevo estado al que se desea mover la orden (Ej: "EnProceso", "Entregada", "Cancelada").</param>
        /// <returns>
        /// Un resultado <see cref="NotFoundResult"/> si la orden no existe;
        /// de lo contrario, redirecciona a la acción <see cref="Ordenes"/> para actualizar la lista.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Recomendación de buena práctica para POST
        public async Task<IActionResult> CambiarEstado(int idOrden, string estado)
        {
            PreOrden orden = await _ordenService.CambiarEstadoOrden(idOrden, estado);

            if (orden == null)
            {
                // Devolver un código 404 si la orden no se encuentra
                return NotFound();
            }

            // Redirigir a la lista de órdenes para ver la actualización
            return RedirectToAction(nameof(Ordenes));
        }

        // ---------------------------------------------
        // GESTIÓN DE USUARIOS (Redirección)
        // ---------------------------------------------

        /// <summary>
        /// Redirecciona al controlador centralizado para la gestión de usuarios (<c>UsuarioController.Index</c>).
        /// </summary>
        /// <returns>Redirecciona a la acción "Index" del controlador "Usuario".</returns>
        public IActionResult Usuarios()
        {
            return RedirectToAction("Index", "Usuario");
        }
    }
}
