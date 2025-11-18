using Microsoft.AspNetCore.Mvc;
using Appetite_App.Models;
using Appetite_App.Repositories;

namespace Appetite.Controllers
{
    public class OrdenController : Controller
    {
        private readonly IOrdenRepository _ordenRepository;

        public OrdenController(IOrdenRepository ordenRepository)
        {
            _ordenRepository = ordenRepository;
        }

        private bool EsAdministrador()
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");
            return rol == "Administrador";
        }

        public async Task<IActionResult> Index() // Antes 'Ordenes' en AdminController
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            var ordenes = await _ordenRepository.GetAllAsync();
            return View(ordenes);
        }

        public async Task<IActionResult> Detalle(int id) // Antes 'DetalleOrden' en AdminController
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            var orden = await _ordenRepository.GetByIdAsync(id);
            return View(orden);
        }

        // Se pueden añadir aquí acciones para cambiar el estado de la orden (Actualizar) si es necesario.
    }
}
