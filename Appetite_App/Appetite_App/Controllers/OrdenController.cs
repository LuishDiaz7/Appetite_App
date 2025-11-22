using Microsoft.AspNetCore.Mvc;
using Appetite_App.Models;
using Appetite_App.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Appetite_App.Controllers
{
    /// <summary>
    /// Controlador encargado de la visualización y gestión de órdenes de compra.
    /// Este controlador está restringido exclusivamente a usuarios con el rol de Administrador.
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class OrdenController : Controller
    {
        private readonly IOrdenRepository _ordenRepository;

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="OrdenController"/>.
        /// </summary>
        /// <param name="ordenRepository">El repositorio para acceder a la capa de persistencia de las órdenes.</param>
        public OrdenController(IOrdenRepository ordenRepository)
        {
            _ordenRepository = ordenRepository;
        }

        /// <summary>
        /// Muestra la lista de todas las órdenes registradas en el sistema.
        /// </summary>
        /// <returns>Una vista que contiene una colección de objetos <see cref="PreOrden"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<PreOrden> ordenes = await _ordenRepository.GetAllAsync();
            return View(ordenes);
        }

        /// <summary>
        /// Muestra los detalles de una orden específica.
        /// </summary>
        /// <param name="id">El identificador único de la orden que se desea visualizar.</param>
        /// <returns>
        /// La vista con el objeto <see cref="PreOrden"/> si se encuentra la orden;
        /// de lo contrario, retorna un resultado 404 <see cref="NotFoundResult"/>.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            PreOrden? orden = await _ordenRepository.GetByIdAsync(id);
            if (orden == null)
            {
                return NotFound();
            }
            return View(orden);
        }

        // Nota: Las acciones para modificar el estado de la orden (como "CambiarEstado")
        // se manejan idealmente en el AdminController o un servicio dedicado para desacoplamiento.
    }
}