using Microsoft.AspNetCore.Mvc;
using Appetite_App.Models;
using Appetite_App.Repositories;
using Appetite_App.Data.Repositories;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.Extensions.Logging; 
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Appetite.Controllers
{
    [Route("Productos")]
    public class ProductoController : Controller
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProductoController> _logger; // Logger añadido

        public ProductoController(IProductoRepository productoRepository, ICategoriaRepository categoriaRepository, IWebHostEnvironment webHostEnvironment, ILogger<ProductoController> logger)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger; // Inyectar logger
        }

        // Método auxiliar para eliminar un archivo físico
        private void EliminarImagenFisica(string url)
        {
            if (string.IsNullOrEmpty(url) || url.Contains("default")) return;

            try
            {
                // Convierte la URL relativa (ej: /images/productos/x.jpg) a ruta física (ej: C:\path\wwwroot\images\productos\x.jpg)
                string fullPathToDelete = Path.Combine(_webHostEnvironment.WebRootPath, url.TrimStart('/'));

                if (System.IO.File.Exists(fullPathToDelete))
                {
                    System.IO.File.Delete(fullPathToDelete);
                    _logger.LogInformation($"[IMAGEN] Archivo eliminado: {fullPathToDelete}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[IMAGEN ERROR] No se pudo eliminar el archivo: {url}");
            }
        }


        // Lógica central para guardar el archivo en wwwroot/images/productos
        private async Task<string> GuardarArchivoImagen(IFormFile? imagenFile, string? urlActual)
        {
            // Si no se sube un nuevo archivo, devolvemos la URL que ya existía.
            if (imagenFile == null)
            {
                return urlActual ?? string.Empty;
            }

            try
            {
                // CRÍTICO: 1. Eliminar la imagen antigua ANTES de guardar la nueva.
                if (!string.IsNullOrEmpty(urlActual))
                {
                    EliminarImagenFisica(urlActual);
                }

                // 2. Generar nombre de archivo único
                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagenFile.FileName);

                // 3. Definir la ruta física de la carpeta de destino
                var rutaCarpeta = Path.Combine(_webHostEnvironment.WebRootPath, "images", "productos");

                // 🔑 CRÍTICO: Asegurar que la carpeta exista.
                if (!Directory.Exists(rutaCarpeta))
                {
                    Directory.CreateDirectory(rutaCarpeta);
                    _logger.LogInformation($"[IMAGEN] Carpeta creada: {rutaCarpeta}");
                }

                var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                // 4. Guardar el archivo físicamente
                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await imagenFile.CopyToAsync(stream);
                }

                // 5. Devolver la URL RELATIVA para la base de datos
                return $"/images/productos/{nombreArchivo}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[IMAGEN ERROR] Fallo al guardar la imagen.");
                // Si falla el guardado, devuelve la URL antigua para evitar perder la referencia si se sube un nuevo archivo.
                return urlActual ?? string.Empty;
            }
        }


        // =========================================================
        // ACCIÓN INDEX (LISTADO)
        // =========================================================
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var productos = await _productoRepository.GetAllAsync();
            return View("~/Views/Producto/Index.cshtml", productos);
        }

        // =========================================================
        // ACCIÓN CREAR GET
        // =========================================================
        [Authorize(Roles = "Administrador")]
        [HttpGet("Crear")]
        public async Task<IActionResult> Crear()
        {
            ViewBag.Categorias = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _categoriaRepository.GetAllAsync(), "IdCategoria", "Nombre");
            return View("~/Views/Producto/Crear.cshtml");
        }

        // =========================================================
        // ACCIÓN CREAR POST
        // =========================================================
        [Authorize(Roles = "Administrador")]
        [HttpPost("Crear")]
        public async Task<IActionResult> Crear(Producto producto, IFormFile? imagenFile)
        {
            // Si el modelo no es válido (ej. faltan campos requeridos), regresamos.
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Por favor, complete todos los campos requeridos.";
                ViewBag.Categorias = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _categoriaRepository.GetAllAsync(), "IdCategoria", "Nombre", producto.IdCategoria);
                return View("~/Views/Producto/Crear.cshtml", producto);
            }

            try
            {
                // Si no se subió imagen, se asigna la URL por defecto para evitar NULL
                string urlNueva = "/images/default/placeholder.jpg";
                if (imagenFile != null)
                {
                    urlNueva = await GuardarArchivoImagen(imagenFile, null);
                }

                producto.ImagenUrl = urlNueva;

                await _productoRepository.AddAsync(producto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al crear producto: " + ex.Message;
                ViewBag.Categorias = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _categoriaRepository.GetAllAsync(), "IdCategoria", "Nombre", producto.IdCategoria);
                return View("~/Views/Producto/Crear.cshtml", producto);
            }
        }

        // =========================================================
        // ACCIÓN EDITAR GET
        // =========================================================
        [Authorize(Roles = "Administrador")]
        [HttpGet("Editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);
            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var todasLasCategorias = await _categoriaRepository.GetAllAsync();

            ViewBag.Categorias = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                todasLasCategorias,
                "IdCategoria",
                "Nombre",
                producto.IdCategoria
            );

            return View("~/Views/Producto/Editar.cshtml", producto);
        }

        // =========================================================
        // ACCIÓN EDITAR POST
        // =========================================================
        [Authorize(Roles = "Administrador")]
        [HttpPost("Editar/{id}")]
        public async Task<IActionResult> Editar(int id, Producto producto, IFormFile? imagenFile)
        {
            if (id != producto.IdProducto)
            {
                TempData["Error"] = "ID de Producto no coincide.";
                return RedirectToAction(nameof(Index));
            }

            var productoExistente = await _productoRepository.GetByIdAsync(id);
            if (productoExistente == null)
            {
                TempData["Error"] = "Producto no encontrado para actualizar.";
                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido (ej. faltan campos requeridos), regresamos.
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Por favor, complete todos los campos requeridos.";
                ViewBag.Categorias = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _categoriaRepository.GetAllAsync(), "IdCategoria", "Nombre", producto.IdCategoria);
                return View("~/Views/Producto/Editar.cshtml", productoExistente);
            }

            try
            {
                // 1. Manejar la subida de archivos (pasa la URL actual del producto existente)
                string nuevaUrl = await GuardarArchivoImagen(imagenFile, productoExistente.ImagenUrl);

                // 2. Actualizar las propiedades del objeto existente (CRÍTICO)
                productoExistente.Nombre = producto.Nombre;
                productoExistente.Descripcion = producto.Descripcion;
                productoExistente.Precio = producto.Precio;
                productoExistente.Stock = producto.Stock;
                productoExistente.Activo = producto.Activo;
                productoExistente.IdCategoria = producto.IdCategoria;
                productoExistente.ImagenUrl = nuevaUrl; // Asignar la nueva URL

                // 3. Guardar en la BD
                await _productoRepository.UpdateAsync(productoExistente);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar producto: " + ex.Message;
                // Recargar el SelectList si hay error
                ViewBag.Categorias = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _categoriaRepository.GetAllAsync(), "IdCategoria", "Nombre", producto.IdCategoria);
                return View("~/Views/Producto/Editar.cshtml", productoExistente);
            }
        }

        // =========================================================
        // ACCIÓN ELIMINAR POST
        // =========================================================
        [Authorize(Roles = "Administrador")]
        [HttpPost("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var producto = await _productoRepository.GetByIdAsync(id);
                if (producto != null)
                {
                    // Eliminar el archivo físico asociado antes de eliminar la BD
                    EliminarImagenFisica(producto.ImagenUrl);
                }

                await _productoRepository.DeleteAsync(id);
                TempData["Success"] = "Producto eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar producto: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [HttpGet("Detalle/{id}")]
        public async Task<IActionResult> Detalle(int id)
        {
            try
            {
                var producto = await _productoRepository.GetByIdAsync(id);

                if (producto == null)
                {
                    TempData["Error"] = "El producto solicitado no fue encontrado.";
                    return RedirectToAction("Index", "Home");
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el detalle del producto con ID: {ProductId}", id);
                TempData["Error"] = "Ocurrió un error al cargar el detalle del producto.";
                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous] // Importante: Permite el acceso a cualquier usuario
        [HttpGet("Catalogo/{categoryId}")]
        public async Task<IActionResult> Catalogo(int categoryId)
        {
            try
            {
                // 1. Obtener los productos filtrados por la categoría
                var productos = await _productoRepository.GetByCategoryIdAsync(categoryId);

                // 2. Obtener la categoría para el título de la página
                var categoria = await _categoriaRepository.GetByIdAsync(categoryId);
                ViewBag.CategoryName = categoria?.Nombre ?? "Productos";

                // 3. Devolver la vista Catalogo.cshtml con la lista de productos
                return View("~/Views/Producto/Catalogo.cshtml", productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el catálogo de productos por categoría.");
                TempData["Error"] = "Ocurrió un error al cargar los productos.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}