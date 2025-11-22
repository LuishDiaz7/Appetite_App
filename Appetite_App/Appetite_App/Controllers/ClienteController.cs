using Appetite_App.DTOs;
using Appetite_App.Models;
using Appetite_App.Patterns.Decorator;
using Appetite_App.Repositories;
using Appetite_App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; // Para obtener el Id del usuario
using System.Text.Json; // Para manejar la sesión
using System.Threading.Tasks;

namespace Appetite_App.Controllers
{
    /// <summary>
    /// Controlador que maneja la interacción del cliente con el sistema, incluyendo
    /// la visualización del catálogo, la gestión del carrito y la finalización de la compra.
    /// Requiere que el usuario esté autenticado con el rol de Cliente.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class ClienteController : Controller
    {
        private readonly IProductoRepository _productoRepository;
        private readonly OrdenService _ordenService;
        private readonly IUsuarioRepository _usuarioRepository;

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="ClienteController"/>.
        /// </summary>
        /// <param name="productoRepository">El repositorio para acceder a la capa de persistencia de productos.</param>
        /// <param name="ordenService">El servicio que contiene la lógica de negocio para las órdenes (Builder y Observer).</param>
        /// <param name="usuarioRepository">El repositorio para acceder a la capa de persistencia de usuarios.</param>
        public ClienteController(IProductoRepository productoRepository, OrdenService ordenService, IUsuarioRepository usuarioRepository)
        {
            _productoRepository = productoRepository;
            _ordenService = ordenService;
            _usuarioRepository = usuarioRepository;
        }

        // ---------------------------------------------
        // CATÁLOGO DE PRODUCTOS
        // ---------------------------------------------

        /// <summary>
        /// Muestra el catálogo completo de productos disponibles para el cliente.
        /// </summary>
        /// <returns>Una vista que contiene una colección de objetos <see cref="Producto"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Producto> productos = await _productoRepository.GetAllAsync();
            return View(productos);
        }

        // ---------------------------------------------
        // GESTIÓN DEL CARRITO (MÉTODOS PRIVADOS DE SESIÓN)
        // ---------------------------------------------

        /// <summary>
        /// Recupera la lista de ítems del carrito de compras de la sesión HTTP.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="CarritoItemDTO"/>. Retorna una lista vacía si no existe en sesión.</returns>
        private List<CarritoItemDTO> GetCarrito()
        {
            string? carritoJson = HttpContext.Session.GetString("Carrito");
            if (string.IsNullOrEmpty(carritoJson))
            {
                return new List<CarritoItemDTO>();
            }
            // Usa System.Text.Json para deserializar
            return JsonSerializer.Deserialize<List<CarritoItemDTO>>(carritoJson) ?? new List<CarritoItemDTO>();
        }

        /// <summary>
        /// Guarda la lista actual del carrito de compras en la sesión HTTP.
        /// </summary>
        /// <param name="carrito">La lista de objetos <see cref="CarritoItemDTO"/> a guardar.</param>
        private void SaveCarrito(List<CarritoItemDTO> carrito)
        {
            // Usa System.Text.Json para serializar
            HttpContext.Session.SetString("Carrito", JsonSerializer.Serialize(carrito));
        }

        // ---------------------------------------------
        // ACCIONES DEL CARRITO
        // ---------------------------------------------

        /// <summary>
        /// Añade un producto con sus posibles decoradores al carrito de compras en la sesión.
        /// </summary>
        /// <param name="idProducto">El ID del producto base.</param>
        /// <param name="cantidad">La cantidad de unidades.</param>
        /// <param name="quesoExtra">Indica si el Decorador "QuesoExtra" se aplica.</param>
        /// <param name="carneDoble">Indica si el Decorador "CarneDoble" se aplica.</param>
        /// <param name="bebidaGrande">Indica si el Decorador "BebidaGrande" se aplica.</param>
        /// <returns>Redirecciona a la acción <see cref="Carrito"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Buena práctica
        public IActionResult AddToCart(int idProducto, int cantidad, bool quesoExtra, bool carneDoble, bool bebidaGrande)
        {
            List<CarritoItemDTO> carrito = GetCarrito();

            List<string> decoradores = new List<string>();
            if (quesoExtra) decoradores.Add("QuesoExtra");
            if (carneDoble) decoradores.Add("CarneDoble");
            if (bebidaGrande) decoradores.Add("BebidaGrande");

            carrito.Add(new CarritoItemDTO
            {
                IdProducto = idProducto,
                Cantidad = cantidad,
                Decoradores = decoradores
            });

            SaveCarrito(carrito);
            return RedirectToAction(nameof(Carrito));
        }

        /// <summary>
        /// Muestra la vista detallada del carrito de compras, calculando precios finales
        /// utilizando el Patrón Decorator (mediante <see cref="OrdenService"/>).
        /// </summary>
        /// <returns>Una vista que contiene una lista anónima con los detalles de cada ítem (precio, subtotal, descripción).</returns>
        [HttpGet]
        public async Task<IActionResult> Carrito()
        {
            List<CarritoItemDTO> carrito = GetCarrito();
            List<object> detallesCarrito = new List<object>();

            // Calcular detalles y aplicar Decorator
            foreach (CarritoItemDTO item in carrito)
            {
                Producto? productoBase = await _productoRepository.GetByIdAsync(item.IdProducto);
                if (productoBase == null) continue;

                // Usamos el OrdenService para aplicar el patrón Decorator y obtener el precio/descripción final.
                IProductoComponente componente = _ordenService.ConstruirComponente(productoBase, item.Decoradores);

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

            // Calcular el total
            ViewBag.Total = detallesCarrito.Sum(d => (decimal)((dynamic)d).Subtotal);
            return View(detallesCarrito);
        }

        // ---------------------------------------------
        // CHECKOUT Y ÓRDENES
        // ---------------------------------------------

        /// <summary>
        /// Finaliza el proceso de compra. Crea la orden utilizando el Patrón Builder
        /// y activa las acciones post-creación mediante el Patrón Observer.
        /// </summary>
        /// <param name="direccion">La dirección de entrega proporcionada por el cliente.</param>
        /// <returns>Redirecciona a la acción <see cref="MisOrdenes"/> tras crear la orden.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string direccion)
        {
            // Obtener ID del usuario autenticado
            string? userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Auth");
            int idUsuario = int.Parse(userIdString);

            List<CarritoItemDTO> carrito = GetCarrito();
            if (carrito.Count == 0) return RedirectToAction(nameof(Index));

            // 1. Obtener objeto Usuario completo
            Usuario? usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
            if (usuario == null) return Unauthorized();

            // 2. Crear la orden. Esto ejecuta el Builder y el Observer.
            PreOrden nuevaOrden = await _ordenService.CrearOrdenDesdeCarrito(usuario, direccion, carrito);

            // 3. Limpiar carrito y notificar al usuario
            HttpContext.Session.Remove("Carrito");

            TempData["MensajeOrden"] = $"¡Orden #{nuevaOrden.IdOrden} creada con éxito! Se notificó a todos los sistemas.";
            return RedirectToAction(nameof(MisOrdenes));
        }

        /// <summary>
        /// Muestra la lista de órdenes históricas realizadas por el cliente autenticado.
        /// </summary>
        /// <returns>Una vista que contiene una colección de objetos <see cref="PreOrden"/> del usuario actual.</returns>
        [HttpGet]
        public async Task<IActionResult> MisOrdenes()
        {
            // Obtener ID del usuario autenticado
            string? userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Auth");
            int idUsuario = int.Parse(userIdString);

            // Obtener las órdenes
            IEnumerable<PreOrden> ordenes = await _ordenService.GetOrdenesPorUsuario(idUsuario);

            return View(ordenes);
        }
    }
}