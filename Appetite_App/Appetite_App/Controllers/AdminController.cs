using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Appetite_App.Repositories;
using Appetite_App.Models;
using Appetite_App.Services;
using System.Threading.Tasks;

// Restringir este controlador solo a usuarios con el rol 'Administrador'
[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    // 🚨 CAMBIO 1: Eliminamos la dependencia de UserManagement.
    // private readonly UserManagement _userManager; 

    private readonly IOrdenRepository _ordenRepository;
    private readonly OrdenService _ordenService;

    // 🚨 CAMBIO 2: Modificamos el constructor.
    public AdminController(
        // Eliminamos UserManagement userManager
        IOrdenRepository ordenRepository,
        OrdenService ordenService)
    {
        // _userManager = userManager; // Eliminado
        _ordenRepository = ordenRepository;
        _ordenService = ordenService;
    }

    // Página principal de la administración
    public IActionResult Index()
    {
        return View();
    }

    // ---------------------------------------------
    // GESTIÓN DE USUARIOS
    // ---------------------------------------------

    // 🚨 CAMBIO 3: La gestión de usuarios ahora se maneja en UsuarioController.
    // Redireccionamos a la acción centralizada.
    // Opcional: Podrías simplemente eliminar esta acción si el enlace en la vista Admin/Index ya apunta a Usuario/Index.
    public IActionResult Usuarios()
    {
        // Redirigimos al controlador centralizado de usuarios
        return RedirectToAction("Index", "Usuario");
    }

    // ---------------------------------------------
    // GESTIÓN DE ÓRDENES
    // ---------------------------------------------

    // Muestra todas las órdenes
    public async Task<IActionResult> Ordenes()
    {
        var ordenes = await _ordenRepository.GetAllAsync();
        return View(ordenes);
    }

    // Acción para cambiar el estado de la orden (usa OrdenService, que usa OBSERVER)
    [HttpPost]
    public async Task<IActionResult> CambiarEstado(int idOrden, string estado)
    {
        var orden = await _ordenService.CambiarEstadoOrden(idOrden, estado);

        if (orden == null) return NotFound();

        return RedirectToAction(nameof(Ordenes));
    }
}
