using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Appetite_App.Repositories;
using Appetite_App.Models;
using Appetite_App.Services;

// Restringir este controlador solo a usuarios con el rol 'Administrador'
[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly IProductoRepository _productoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly UserManagement _userManager;
    private readonly IOrdenRepository _ordenRepository;
    private readonly OrdenService _ordenService;

    public AdminController(
        IProductoRepository productoRepository,
        ICategoriaRepository categoriaRepository,
        UserManagement userManager,
        IOrdenRepository ordenRepository,
        OrdenService ordenService)
    {
        _productoRepository = productoRepository;
        _categoriaRepository = categoriaRepository;
        _userManager = userManager;
        _ordenRepository = ordenRepository;
        _ordenService = ordenService;
    }

    // Página principal de la administración
    public IActionResult Index()
    {
        return View();
    }

    // ---------------------------------------------
    // GESTIÓN DE PRODUCTOS
    // ---------------------------------------------

    // Muestra todos los productos
    public async Task<IActionResult> Productos()
    {
        var productos = await _productoRepository.GetAllAsync();
        return View(productos);
    }

    // GET: Muestra formulario para crear/editar
    public async Task<IActionResult> EditarProducto(int id = 0)
    {
        ViewBag.Categorias = await _categoriaRepository.GetAllAsync();
        if (id == 0)
        {
            return View(new Producto()); // Nuevo producto
        }

        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto == null) return NotFound();

        return View(producto);
    }

    // POST: Guarda o actualiza un producto
    [HttpPost]
    public async Task<IActionResult> GuardarProducto(Producto producto)
    {
        if (ModelState.IsValid)
        {
            if (producto.IdProducto == 0)
            {
                await _productoRepository.AddAsync(producto);
            }
            else
            {
                await _productoRepository.UpdateAsync(producto);
            }
            return RedirectToAction(nameof(Productos));
        }
        ViewBag.Categorias = await _categoriaRepository.GetAllAsync();
        return View("EditarProducto", producto);
    }

    // Página principal de la administración (Ajustamos para el botón 'Panel Admin')
    public IActionResult Dashboard()
    {
        // Redirigir al listado de productos, que es el punto de inicio más útil para el admin.
        return RedirectToAction(nameof(Productos));
    }

    // ... (resto de Index(), Productos() )

    // NUEVA ACCIÓN: Crea un endpoint limpio para la creación de productos (Resuelve el 404)
    public IActionResult CrearProducto()
    {
        // Redirige a EditarProducto, pasándole 0 como ID, lo que activa la lógica de "nuevo producto"
        return RedirectToAction(nameof(EditarProducto), new { id = 0 });
    }

    // ---------------------------------------------
    // GESTIÓN DE USUARIOS
    // ---------------------------------------------

    // Muestra todos los usuarios (usa UserManagement)
    public async Task<IActionResult> Usuarios()
    {
        var usuarios = await _userManager.ObtenerTodos();
        return View(usuarios);
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
