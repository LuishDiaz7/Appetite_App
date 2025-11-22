using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Appetite_App.Models;
using Appetite_App.Patterns.Decorator;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appetite_App.Repositories;

namespace Appetite_App.Controllers
{
    /// <summary>
    /// Gestiona la lógica del carrito de compras, incluyendo la adición, visualización y eliminación de ítems.
    /// Es la capa donde se aplica el Patrón Decorator para calcular el precio final de los productos con adiciones.
    /// </summary>
    public class CarritoController : Controller
    {
        private readonly IProductoRepository _productoRepository;

        /// <summary>
        /// Clave constante utilizada para almacenar y recuperar la lista del carrito de compras de la sesión HTTP.
        /// </summary>
        private const string CarritoSessionKey = "CarritoDeCompras";

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="CarritoController"/>.
        /// </summary>
        /// <param name="productoRepository">El repositorio para acceder a la capa de persistencia de productos.</param>
        public CarritoController(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        // ---------------------------------------------
        // VISTA DEL CARRITO
        // ---------------------------------------------

        /// <summary>
        /// Muestra la vista con el contenido actual del carrito de compras almacenado en la sesión.
        /// </summary>
        /// <returns>La vista del carrito, que contiene una lista de objetos <see cref="ItemCarrito"/>.</returns>
        [HttpGet]
        public IActionResult Index()
        {
            List<ItemCarrito> carrito = GetCarritoFromSession();
            return View(carrito);
        }

        // ---------------------------------------------
        // AÑADIR AL CARRITO (Patrón Decorator)
        // ---------------------------------------------

        /// <summary>
        /// Añade un producto al carrito de compras, aplicando dinámicamente adiciones (Decorators)
        /// para modificar su precio y descripción final.
        /// </summary>
        /// <param name="idProducto">El identificador único del producto base a agregar.</param>
        /// <param name="cantidad">La cantidad de unidades del producto base.</param>
        /// <param name="quesoExtra">Indica si se aplica la adición de queso extra.</param>
        /// <param name="carneDoble">Indica si se aplica la adición de carne doble.</param>
        /// <param name="bebidaGrande">Indica si se aplica la adición de bebida grande.</param>
        /// <returns>Redirecciona a la acción <see cref="Index"/> del carrito tras añadir el ítem.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(
            int idProducto,
            int cantidad,
            bool quesoExtra,
            bool carneDoble,
            bool bebidaGrande)
        {
            Producto productoBaseDb = await _productoRepository.GetByIdAsync(idProducto);

            if (productoBaseDb == null || cantidad <= 0)
            {
                // Manejar error o redirigir
                return RedirectToAction("Catalogo", "Producto");
            }

            // 1. Inicializar el componente base (Patrón Decorator)
            IProductoComponente componenteDecorado = new ProductoConcreto(productoBaseDb);

            // 2. Aplicar los Decoradores Condicionalmente
            if (quesoExtra)
            {
                componenteDecorado = new QuesoExtraDecorator(componenteDecorado);
            }

            if (carneDoble)
            {
                componenteDecorado = new CarneDobleDecorator(componenteDecorado);
            }

            if (bebidaGrande)
            {
                componenteDecorado = new BebidaGrandeDecorator(componenteDecorado);
            }

            // 3. Crear el ItemCarrito con los valores finales
            decimal precioUnitarioFinal = componenteDecorado.GetPrecio();
            string descripcionFinal = componenteDecorado.GetDescripcion();

            ItemCarrito newItem = new ItemCarrito
            {
                IdProducto = idProducto,
                NombreProducto = productoBaseDb.Nombre,
                Cantidad = cantidad,
                PrecioBaseUnitario = productoBaseDb.Precio,
                PrecioTotal = precioUnitarioFinal * cantidad,
                // Nota: La descripción de los extras ahora está incrustada en DescripcionCompleta por el Decorator
                DescripcionCompleta = descripcionFinal
            };

            // 4. Guardar el nuevo item en la sesión
            AddItemToSession(newItem);

            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------
        // ELIMINAR DEL CARRITO
        // ---------------------------------------------

        /// <summary>
        /// Elimina un ítem del carrito de compras basándose en su identificador de producto.
        /// </summary>
        /// <param name="id">El identificador único del producto a eliminar del carrito.</param>
        /// <returns>Redirecciona a la acción <see cref="Index"/> del carrito tras la eliminación.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int id)
        {
            List<ItemCarrito> carrito = GetCarritoFromSession();
            ItemCarrito itemToRemove = carrito.FirstOrDefault(i => i.IdProducto == id);

            if (itemToRemove != null)
            {
                carrito.Remove(itemToRemove);
                TempData["Success"] = $"Producto '{itemToRemove.NombreProducto}' eliminado del carrito.";

                // Actualizar la sesión con la lista modificada
                HttpContext.Session.SetString(CarritoSessionKey, JsonConvert.SerializeObject(carrito));
            }
            else
            {
                TempData["Error"] = "El producto no se encontró en el carrito.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------
        // MÉTODOS PRIVADOS DE GESTIÓN DE SESIÓN
        // ---------------------------------------------

        /// <summary>
        /// Recupera la lista del carrito de compras de la sesión HTTP.
        /// Si no existe un carrito en la sesión, retorna una lista vacía.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="ItemCarrito"/>.</returns>
        private List<ItemCarrito> GetCarritoFromSession()
        {
            string json = HttpContext.Session.GetString(CarritoSessionKey);
            return json == null
                ? new List<ItemCarrito>()
                : JsonConvert.DeserializeObject<List<ItemCarrito>>(json) ?? new List<ItemCarrito>();
        }

        /// <summary>
        /// Añade un <see cref="ItemCarrito"/> a la lista en la sesión HTTP.
        /// </summary>
        /// <param name="item">El ítem del carrito a agregar.</param>
        private void AddItemToSession(ItemCarrito item)
        {
            List<ItemCarrito> carrito = GetCarritoFromSession();
            carrito.Add(item);
            HttpContext.Session.SetString(CarritoSessionKey, JsonConvert.SerializeObject(carrito));
        }
    }
}
