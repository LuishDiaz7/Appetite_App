using Appetite_App.Services; 
using Microsoft.AspNetCore.Mvc;
using Appetite_App.Repositories;

public class HomeController : Controller
{
    private readonly IProductoService _productoService;
    private readonly ICategoriaRepository _categoriaRepository;

    // Inyección de Dependencias
    public HomeController(IProductoService productoService, ICategoriaRepository categoriaRepository)
    {
        _productoService = productoService;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<IActionResult> Index()
    {
        var categorias = await _categoriaRepository.GetAllWithProductsAsync();

        return View(categorias);
    }

    // ... Otros métodos del controlador ...
}
