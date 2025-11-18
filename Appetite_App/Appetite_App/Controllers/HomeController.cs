using Appetite_App.Services; 
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly IProductoService _productoService;

    // Inyección de Dependencias
    public HomeController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    public async Task<IActionResult> Index()
    {
        var productos = await _productoService.GetAllProductosAsync();

        // Agrupar los productos por nombre de Categoría
        // Usamos el resultado de un GroupBy como modelo de la vista
        var model = productos.GroupBy(p => p.Categoria.Nombre);

        return View(model);
    }

    // ... Otros métodos del controlador ...
}
