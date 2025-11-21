using Microsoft.AspNetCore.Mvc;
using Appetite_App.Models;
using Appetite_App.Repositories;
using Microsoft.AspNetCore.Authorization; // 🚨 NECESARIO para [Authorize]
using System.Threading.Tasks;

namespace Appetite.Controllers
{
    // 🚨 CAMBIO 1: Aplicar el atributo de autorización a nivel de controlador
    [Authorize(Roles = "Administrador")]
    public class OrdenController : Controller
    {
        private readonly IOrdenRepository _ordenRepository;

        public OrdenController(IOrdenRepository ordenRepository)
        {
            _ordenRepository = ordenRepository;
        }

        // 🚨 CAMBIO 2: ELIMINAMOS el método EsAdministrador()
        /*
        private bool EsAdministrador()
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");
            return rol == "Administrador";
        }
        */

        public async Task<IActionResult> Index() // Muestra todas las órdenes para el Admin
        {
            // 🚨 CAMBIO 3: ELIMINAMOS la verificación manual
            // if (!EsAdministrador())
            //     return RedirectToAction("Login", "Auth");

            var ordenes = await _ordenRepository.GetAllAsync();
            return View(ordenes);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            // 🚨 CAMBIO 3: ELIMINAMOS la verificación manual
            // if (!EsAdministrador())
            //     return RedirectToAction("Login", "Auth");

            var orden = await _ordenRepository.GetByIdAsync(id);
            if (orden == null)
            {
                return NotFound();
            }
            return View(orden);
        }

        // Se pueden añadir aquí acciones para cambiar el estado de la orden (Actualizar) si es necesario.
    }
}
