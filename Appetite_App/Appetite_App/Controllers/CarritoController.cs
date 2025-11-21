using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Appetite_App.Models;
using Appetite_App.Patterns.Decorator;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appetite_App.Repositories;


// Implementación del controlador
public class CarritoController : Controller
{
    private readonly IProductoRepository _productoRepository;
    private const string CarritoSessionKey = "CarritoDeCompras";

    // Reemplaza IProductoRepository por tu contexto de Entity Framework o servicio de datos real
    public CarritoController(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    // Muestra el carrito
    public IActionResult Index()
    {
        var carrito = GetCarritoFromSession();
        return View(carrito);
    }

    // 🔑 ACCIÓN CRÍTICA: Recibe datos de Detalle.cshtml y aplica el Decorator
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(
        int idProducto,
        int cantidad,
        bool quesoExtra,
        bool carneDoble,
        bool bebidaGrande)
    {
        // 1. Obtener el producto base de la DB (necesitas el precio base)
        var productoBaseDb = await _productoRepository.GetByIdAsync(idProducto);

        if (productoBaseDb == null || cantidad <= 0)
        {
            return RedirectToAction("Index", "Home");
        }

        // 2. Inicializar el componente base (Patrón Decorator)
        IProductoComponente componenteDecorado = new ProductoConcreto(productoBaseDb);

        // Lista para guardar las descripciones de los extras
        var descripcionExtras = new List<string>();

        // 3. Aplicar los Decoradores Condicionalmente
        if (quesoExtra)
        {
            componenteDecorado = new QuesoExtraDecorator(componenteDecorado);
            descripcionExtras.Add("Queso Extra");
        }

        if (carneDoble)
        {
            componenteDecorado = new CarneDobleDecorator(componenteDecorado);
            descripcionExtras.Add("Carne Doble");
        }

        if (bebidaGrande)
        {
            componenteDecorado = new BebidaGrandeDecorator(componenteDecorado);
            descripcionExtras.Add("Tamaño Grande");
        }

        // 4. Calcular el precio y la descripción final usando el Decorator
        decimal precioUnitarioFinal = componenteDecorado.GetPrecio();
        string descripcionFinal = componenteDecorado.GetDescripcion(); // Contiene el nombre del producto + todos los extras

        // 5. Crear el ItemCarrito para almacenar
        var newItem = new ItemCarrito
        {
            IdProducto = idProducto,
            NombreProducto = productoBaseDb.Nombre,
            Cantidad = cantidad,
            PrecioBaseUnitario = productoBaseDb.Precio,
            PrecioTotal = precioUnitarioFinal * cantidad,
            DescripcionExtras = descripcionExtras,
            DescripcionCompleta = descripcionFinal
        };

        // 6. Guardar el nuevo item en la sesión
        AddItemToSession(newItem);

        return RedirectToAction("Index");
    }

    // 7. Métodos Auxiliares para manejo de Sesión (usando System.Text.Json o Newtonsoft)
    private List<ItemCarrito> GetCarritoFromSession()
    {
        var json = HttpContext.Session.GetString(CarritoSessionKey);
        return json == null ? new List<ItemCarrito>() : JsonConvert.DeserializeObject<List<ItemCarrito>>(json);
    }

    private void AddItemToSession(ItemCarrito item)
    {
        var carrito = GetCarritoFromSession();
        carrito.Add(item);
        HttpContext.Session.SetString(CarritoSessionKey, JsonConvert.SerializeObject(carrito));
    }
}
