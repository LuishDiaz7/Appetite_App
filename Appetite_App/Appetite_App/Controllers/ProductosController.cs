using Appetite_App.Models;
using Appetite_App.Repositories;
using Appetite_App.Services; 
using Microsoft.AspNetCore.Mvc;

public class ProductosController : Controller
{
    private readonly IProductoRepository _productoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public ProductosController(IProductoRepository productoRepository, ICategoriaRepository categoriaRepository)
    {
        _productoRepository = productoRepository;
        _categoriaRepository = categoriaRepository;
    }

    // Acción para mostrar los productos de una categoría específica
    [HttpGet]
    public async Task<IActionResult> Categoria(int id)
    {
        // 1. Obtener la Categoría por ID
        var categoria = await _categoriaRepository.GetByIdAsync(id);
        if (categoria == null)
        {
            return NotFound();
        }

        // 2. Obtener los productos que pertenecen a esa Categoría
        // Nota: Necesitas añadir un método en IProductoRepository/ProductoRepository
        // para buscar por ID de Categoría.
        var productos = await _productoRepository.GetByCategoryIdAsync(id);

        ViewData["Title"] = categoria.Nombre;
        ViewData["CategoriaNombre"] = categoria.Nombre; // Usaremos esto en la vista

        return View(productos); // Usaremos la vista 'Categoria.cshtml'
    }
}
