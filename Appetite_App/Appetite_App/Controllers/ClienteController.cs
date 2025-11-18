using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Appetite_App.Services;
using Appetite_App.Repositories;
using Appetite_App.Models;
using Appetite_App.DTOs;
using System.Security.Claims; // Para obtener el Id del usuario
using System.Text.Json; // Para manejar la sesión

[Authorize(Roles = "Cliente")]
public class ClienteController : Controller
{
    private readonly IProductoRepository _productoRepository;
    private readonly OrdenService _ordenService;
    private readonly IUsuarioRepository _usuarioRepository;

    public ClienteController(IProductoRepository productoRepository, OrdenService ordenService, IUsuarioRepository usuarioRepository)
    {
        _productoRepository = productoRepository;
        _ordenService = ordenService;
        _usuarioRepository = usuarioRepository;
    }

    // ---------------------------------------------
    // CATÁLOGO DE PRODUCTOS (Usa el Patrón DECORATOR en la vista)
    // ---------------------------------------------
    public async Task<IActionResult> Index()
    {
        var productos = await _productoRepository.GetAllAsync();
        return View(productos);
    }

    // ---------------------------------------------
    // GESTIÓN DEL CARRITO (Usa la sesión)
    // ---------------------------------------------

    private List<CarritoItemDTO> GetCarrito()
    {
        var carritoJson = HttpContext.Session.GetString("Carrito");
        if (string.IsNullOrEmpty(carritoJson))
        {
            return new List<CarritoItemDTO>();
        }
        return JsonSerializer.Deserialize<List<CarritoItemDTO>>(carritoJson) ?? new List<CarritoItemDTO>();
    }

    private void SaveCarrito(List<CarritoItemDTO> carrito)
    {
        HttpContext.Session.SetString("Carrito", JsonSerializer.Serialize(carrito));
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int idProducto, int cantidad, bool quesoExtra, bool carneDoble, bool bebidaGrande)
    {
        var carrito = GetCarrito();

        var decoradores = new List<string>();
        if (quesoExtra) decoradores.Add("QuesoExtra");
        if (carneDoble) decoradores.Add("CarneDoble");
        if (bebidaGrande) decoradores.Add("BebidaGrande");

        // Simplemente agregamos al carrito con los decoradores seleccionados.
        carrito.Add(new CarritoItemDTO
        {
            IdProducto = idProducto,
            Cantidad = cantidad,
            Decoradores = decoradores
        });

        SaveCarrito(carrito);
        return RedirectToAction(nameof(Carrito));
    }

    public async Task<IActionResult> Carrito()
    {
        var carrito = GetCarrito();
        var detallesCarrito = new List<object>();

        // Usar el OrdenService para aplicar el Patrón Decorator y calcular los precios
        foreach (var item in carrito)
        {
            var productoBase = await _productoRepository.GetByIdAsync(item.IdProducto);
            if (productoBase == null) continue;

            // Simulación del Builder/Decorator para obtener el precio/descripción final
            // Usaremos el mismo patrón que el OrdenService para la vista:
            var componente = _ordenService.ConstruirComponente(productoBase, item.Decoradores);

            detallesCarrito.Add(new
            {
                ProductoBase = productoBase.Nombre,
                Cantidad = item.Cantidad,
                PrecioUnitario = componente.GetPrecio(),
                Subtotal = componente.GetPrecio() * item.Cantidad,
                Descripcion = componente.GetDescripcion(),
                Decoradores = item.Decoradores
            });
        }

        ViewBag.Total = detallesCarrito.Sum(d => (decimal)((dynamic)d).Subtotal);
        return View(detallesCarrito);
    }

    // ---------------------------------------------
    // CHECKOUT (Usa los Patrones BUILDER y OBSERVER)
    // ---------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Checkout(string direccion)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Auth");
        int idUsuario = int.Parse(userIdString);

        var carrito = GetCarrito();
        if (carrito.Count == 0) return RedirectToAction(nameof(Index));

        // 1. Obtener objeto Usuario completo (necesario para el Builder)
        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
        if (usuario == null) return Unauthorized();

        // 2. Llamar al servicio con el método correcto (CrearOrdenDesdeCarrito)
        PreOrden nuevaOrden = await _ordenService.CrearOrdenDesdeCarrito(usuario, direccion, carrito);

        // ... (el resto del método Checkout)
        // Limpiar carrito después de la orden
        HttpContext.Session.Remove("Carrito");

        TempData["MensajeOrden"] = $"¡Orden #{nuevaOrden.IdOrden} creada con éxito! Se notificó a todos los sistemas.";
        return RedirectToAction(nameof(MisOrdenes));
    }

    // Muestra las órdenes del cliente (Usa el Patrón Observer indirectamente)
    public async Task<IActionResult> MisOrdenes()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Auth");
        int idUsuario = int.Parse(userIdString);

        // Simulación: Obtener las órdenes del usuario
        var ordenes = await _ordenService.GetOrdenesPorUsuario(idUsuario);

        return View(ordenes);
    }
}
