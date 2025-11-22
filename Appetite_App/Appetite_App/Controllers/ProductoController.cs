using Microsoft.AspNetCore.Mvc;
using Appetite_App.Models;
using Appetite_App.Repositories;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering; // Necesario para SelectList

namespace Appetite_App.Controllers
{
    /// <summary>
    /// Controlador que gestiona la lógica de productos, incluyendo el CRUD administrativo
    /// y las acciones de catálogo/detalle visibles para todos los usuarios.
    /// Las rutas de acción se definen explícitamente usando atributos <c>[Route]</c> y <c>[HttpGet]/[HttpPost]</c>.
    /// </summary>
    [Route("Productos")]
    public class ProductoController : Controller
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProductoController> _logger;

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="ProductoController"/>.
        /// </summary>
        /// <param name="productoRepository">El repositorio para acceder a la capa de persistencia de productos.</param>
        /// <param name="categoriaRepository">El repositorio para acceder a la capa de persistencia de categorías.</param>
        /// <param name="webHostEnvironment">Proporciona información sobre el entorno de alojamiento web (ej. ruta raíz).</param>
        /// <param name="logger">El registrador (logger) para escribir información y errores.</param>
        public ProductoController(IProductoRepository productoRepository, ICategoriaRepository categoriaRepository, IWebHostEnvironment webHostEnvironment, ILogger<ProductoController> logger)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        // ---------------------------------------------
        // MÉTODOS PRIVADOS DE MANEJO DE ARCHIVOS
        // ---------------------------------------------

        /// <summary>
        /// Intenta eliminar el archivo de imagen físico de la carpeta <c>wwwroot</c> usando su URL relativa.
        /// </summary>
        /// <param name="url">La URL relativa del archivo a eliminar (ej: /images/productos/x.jpg).</param>
        private void EliminarImagenFisica(string? url)
        {
            if (string.IsNullOrEmpty(url) || url.Contains("default", StringComparison.OrdinalIgnoreCase)) return;

            try
            {
                // Convierte la URL relativa a la ruta física del servidor
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

        /// <summary>
        /// Guarda un archivo de imagen en <c>wwwroot/images/productos</c>, asegurando un nombre único,
        /// y elimina la imagen anterior si se provee una <c>urlActual</c>.
        /// </summary>
        /// <param name="imagenFile">El archivo de imagen subido por el usuario. Puede ser nulo.</param>
        /// <param name="urlActual">La URL relativa de la imagen existente. Puede ser nula o vacía.</param>
        /// <returns>La URL relativa de la nueva imagen guardada o la <c>urlActual</c> si no se subió una nueva imagen.</returns>
        private async Task<string> GuardarArchivoImagen(IFormFile? imagenFile, string? urlActual)
        {
            if (imagenFile == null)
            {
                return urlActual ?? string.Empty;
            }

            try
            {
                // 1. Eliminar la imagen antigua ANTES de guardar la nueva.
                if (!string.IsNullOrEmpty(urlActual))
                {
                    EliminarImagenFisica(urlActual);
                }

                // 2. Generar nombre de archivo único
                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagenFile.FileName);

                // 3. Definir la ruta física de la carpeta de destino
                string rutaCarpeta = Path.Combine(_webHostEnvironment.WebRootPath, "images", "productos");

                // 4. Asegurar que la carpeta exista.
                if (!Directory.Exists(rutaCarpeta))
                {
                    Directory.CreateDirectory(rutaCarpeta);
                    _logger.LogInformation($"[IMAGEN] Carpeta creada: {rutaCarpeta}");
                }

                string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                // 5. Guardar el archivo físicamente
                using (FileStream stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await imagenFile.CopyToAsync(stream);
                }

                // 6. Devolver la URL RELATIVA para la base de datos
                return $"/images/productos/{nombreArchivo}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[IMAGEN ERROR] Fallo al guardar la imagen.");
                // Si falla el guardado, devuelve la URL antigua para no perder la referencia.
                return urlActual ?? string.Empty;
            }
        }


        // ---------------------------------------------
        // ACCIONES ADMINISTRATIVAS (CRUD)
        // ---------------------------------------------

        /// <summary>
        /// Muestra el listado de todos los productos (vista de administrador).
        /// </summary>
        /// <returns>La vista de índice con una colección de productos.</returns>
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Producto> productos = await _productoRepository.GetAllAsync();
            return View("~/Views/Producto/Index.cshtml", productos);
        }

        /// <summary>
        /// Muestra el formulario para crear un nuevo producto.
        /// Carga dinámicamente la lista de categorías para el SelectList.
        /// </summary>
        /// <returns>La vista del formulario de creación.</returns>
        [Authorize(Roles = "Administrador")]
        [HttpGet("Crear")]
        public async Task<IActionResult> Crear()
        {
            IEnumerable<Categoria> categorias = await _categoriaRepository.GetAllAsync();
            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Nombre");
            return View("~/Views/Producto/Crear.cshtml");
        }

        /// <summary>
        /// Procesa los datos enviados para crear un nuevo producto, incluyendo la subida de la imagen.
        /// </summary>
        /// <param name="producto">El objeto <see cref="Producto"/> con los datos del formulario.</param>
        /// <param name="imagenFile">El archivo de imagen subido.</param>
        /// <returns>Redirecciona a la acción <see cref="Index"/> si es exitoso; de lo contrario, regresa a la vista de creación con errores.</returns>
        [Authorize(Roles = "Administrador")]
        [HttpPost("Crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Producto producto, IFormFile? imagenFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Por favor, complete todos los campos requeridos.";
                ViewBag.Categorias = new SelectList(await _categoriaRepository.GetAllAsync(), "IdCategoria", "Nombre", producto.IdCategoria);
                return View("~/Views/Producto/Crear.cshtml", producto);
            }

            try
            {
                // Asigna URL por defecto si no hay imagen, o guarda la nueva.
                string urlNueva = "/images/default/placeholder.jpg";
                if (imagenFile != null)
                {
                    urlNueva = await GuardarArchivoImagen(imagenFile, null);
                }

                producto.ImagenUrl = urlNueva;

                await _productoRepository.AddAsync(producto);
                TempData["Success"] = "Producto creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la creación del producto.");
                ViewBag.Error = $"Error al crear producto: {ex.Message}";
                ViewBag.Categorias = new SelectList(await _categoriaRepository.GetAllAsync(), "IdCategoria", "Nombre", producto.IdCategoria);
                return View("~/Views/Producto/Crear.cshtml", producto);
            }
        }

        /// <summary>
        /// Muestra el formulario para editar un producto existente.
        /// </summary>
        /// <param name="id">El identificador único del producto a editar.</param>
        /// <returns>La vista de edición con el producto precargado o una redirección si no se encuentra.</returns>
        [Authorize(Roles = "Administrador")]
        [HttpGet("Editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            Producto? producto = await _productoRepository.GetByIdAsync(id);
            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            IEnumerable<Categoria> todasLasCategorias = await _categoriaRepository.GetAllAsync();

            ViewBag.Categorias = new SelectList(
                todasLasCategorias,
                "IdCategoria",
                "Nombre",
                producto.IdCategoria
            );

            return View("~/Views/Producto/Editar.cshtml", producto);
        }

        /// <summary>
        /// Procesa los datos enviados para actualizar un producto existente, manejando la sustitución de la imagen.
        /// </summary>
        /// <param name="id">El identificador único del producto a actualizar.</param>
        /// <param name="producto">El objeto <see cref="Producto"/> con los datos actualizados.</param>
        /// <param name="imagenFile">El nuevo archivo de imagen subido (opcional).</param>
        /// <returns>Redirecciona a la acción <see cref="Index"/> si es exitoso; de lo contrario, regresa a la vista de edición con errores.</returns>
        [Authorize(Roles = "Administrador")]
        [HttpPost("Editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Producto producto, IFormFile? imagenFile)
        {
            if (id != producto.IdProducto)
            {
                TempData["Error"] = "ID de Producto no coincide.";
                return RedirectToAction(nameof(Index));
            }

            Producto? productoExistente = await _productoRepository.GetByIdAsync(id);
            if (productoExistente == null)
            {
                TempData["Error"] = "Producto no encontrado para actualizar.";
                return RedirectToAction(nameof(Index));
            }

            // Validación de ModelState (requerido para recargar ViewBags si falla)
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Por favor, complete todos los campos requeridos.";
                ViewBag.Categorias = new SelectList(await _categoriaRepository.GetAllAsync(), "IdCategoria", "Nombre", producto.IdCategoria);
                return View("~/Views/Producto/Editar.cshtml", productoExistente);
            }

            try
            {
                // 1. Manejar la subida de archivos (pasa la URL actual para la eliminación)
                string nuevaUrl = await GuardarArchivoImagen(imagenFile, productoExistente.ImagenUrl);

                // 2. Actualizar solo las propiedades necesarias en el objeto existente
                productoExistente.Nombre = producto.Nombre;
                productoExistente.Descripcion = producto.Descripcion;
                productoExistente.Precio = producto.Precio;
                productoExistente.Stock = producto.Stock;
                productoExistente.Activo = producto.Activo;
                productoExistente.IdCategoria = producto.IdCategoria;
                productoExistente.ImagenUrl = nuevaUrl;

                // 3. Guardar en la BD
                await _productoRepository.UpdateAsync(productoExistente);
                TempData["Success"] = "Producto actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la actualización del producto con ID: {ProductId}", id);
                ViewBag.Error = $"Error al actualizar producto: {ex.Message}";
                ViewBag.Categorias = new SelectList(await _categoriaRepository.GetAllAsync(), "IdCategoria", "Nombre", producto.IdCategoria);
                return View("~/Views/Producto/Editar.cshtml", productoExistente);
            }
        }

        /// <summary>
        /// Elimina un producto y el archivo de imagen físico asociado.
        /// </summary>
        /// <param name="id">El identificador único del producto a eliminar.</param>
        /// <returns>Redirecciona a la acción <see cref="Index"/>.</returns>
        [Authorize(Roles = "Administrador")]
        [HttpPost("Eliminar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                Producto? producto = await _productoRepository.GetByIdAsync(id);
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
                _logger.LogError(ex, "Error al eliminar el producto con ID: {ProductId}", id);
                TempData["Error"] = $"Error al eliminar producto: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------
        // ACCIONES PÚBLICAS (CATÁLOGO)
        // ---------------------------------------------

        /// <summary>
        /// Muestra el detalle de un producto específico.
        /// </summary>
        /// <param name="id">El identificador único del producto.</param>
        /// <returns>La vista de detalle con el producto o una redirección al home si no se encuentra.</returns>
        [AllowAnonymous]
        [HttpGet("Detalle/{id}")]
        public async Task<IActionResult> Detalle(int id)
        {
            try
            {
                Producto? producto = await _productoRepository.GetByIdAsync(id);

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

        /// <summary>
        /// Muestra el catálogo de productos filtrados por una categoría específica.
        /// </summary>
        /// <param name="categoryId">El identificador único de la categoría a filtrar.</param>
        /// <returns>La vista de catálogo con los productos filtrados.</returns>
        [AllowAnonymous]
        [HttpGet("Catalogo/{categoryId}")]
        public async Task<IActionResult> Catalogo(int categoryId)
        {
            try
            {
                // 1. Obtener los productos filtrados por la categoría
                IEnumerable<Producto> productos = await _productoRepository.GetByCategoryIdAsync(categoryId);

                // 2. Obtener la categoría para el título de la página
                Categoria? categoria = await _categoriaRepository.GetByIdAsync(categoryId);
                ViewBag.CategoryName = categoria?.Nombre ?? "Productos";

                // 3. Devolver la vista Catalogo.cshtml con la lista de productos
                return View("~/Views/Producto/Catalogo.cshtml", productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el catálogo de productos por categoría: {CategoryId}", categoryId);
                TempData["Error"] = "Ocurrió un error al cargar los productos.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}