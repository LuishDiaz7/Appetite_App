using Microsoft.AspNetCore.Mvc;
using Appetite_App.Models;
using Appetite_App.Repositories;
using Microsoft.AspNetCore.Hosting; 
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http; // Necesario para IFormFile

namespace Appetite.Controllers
{
    // Este controlador ahora manejará la gestión de categorías
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepository _categoriaRepository;
        // NUEVO: Inyección del entorno de hosting para manejar rutas físicas (wwwroot)
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Constructor modificado para inyectar IWebHostEnvironment
        public CategoriaController(ICategoriaRepository categoriaRepository, IWebHostEnvironment webHostEnvironment)
        {
            _categoriaRepository = categoriaRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // Método de autenticación basado en Sesión (según su implementación)
        private bool EsAdministrador()
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");
            return rol == "Administrador";
        }

        // Lógica central para guardar el archivo en wwwroot/images/categorias
        private async Task<string?> GuardarArchivoImagen(IFormFile? imagenFile, string? existingImagePath)
        {
            if (imagenFile == null)
            {
                // Si no se sube un nuevo archivo, mantiene la ruta existente.
                return existingImagePath;
            }

            // 1. Eliminar la imagen anterior si existe (para evitar archivos huérfanos)
            if (!string.IsNullOrEmpty(existingImagePath))
            {
                // existingImagePath debe ser una ruta relativa como /images/categorias/archivo.jpg
                string fullPathToDelete = Path.Combine(_webHostEnvironment.WebRootPath, existingImagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPathToDelete))
                {
                    System.IO.File.Delete(fullPathToDelete);
                }
            }

            // 2. Definir la carpeta de subidas
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "categorias");

            // Asegurar que la carpeta exista
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 3. Generar un nombre único y guardar
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imagenFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imagenFile.CopyToAsync(fileStream);
            }

            // 4. Devolver la ruta relativa a wwwroot
            return $"/images/categorias/{uniqueFileName}";
        }


        public async Task<IActionResult> Index()
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            var categorias = await _categoriaRepository.GetAllAsync();
            return View(categorias);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        // CAMBIO CRÍTICO: Recibir el archivo IFormFile
        public async Task<IActionResult> Crear(Categoria categoria, IFormFile? imagenFile)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            // VALIDACIÓN: Aunque no siempre se usa ModelState.IsValid en el POST
            // con try-catch, es bueno tenerlo. Aquí asumiremos que el try-catch
            // maneja la mayoría de los errores de datos.

            try
            {
                // NUEVO: Guarda la imagen y actualiza la URL en el modelo
                categoria.ImagenUrl = await GuardarArchivoImagen(imagenFile, null);

                await _categoriaRepository.AddAsync(categoria);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // IMPORTANTE: Use TempData para errores si hace Redirect, o ViewBag si retorna la View
                ViewBag.Error = "Error al crear categoría: " + ex.Message;
                return View(categoria);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
            {
                // Manejo si la categoría no existe
                TempData["Error"] = "Categoría no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        [HttpPost]
        // CAMBIO CRÍTICO: Recibir el archivo IFormFile y el Id
        public async Task<IActionResult> Editar(int id, Categoria categoria, IFormFile? imagenFile)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            if (id != categoria.IdCategoria)
            {
                TempData["Error"] = "ID de Categoría no coincide.";
                return RedirectToAction(nameof(Index));
            }

            // Re-obtener la categoría de la BD para obtener la ruta de la imagen existente
            var categoriaExistente = await _categoriaRepository.GetByIdAsync(id);
            if (categoriaExistente == null)
            {
                TempData["Error"] = "Categoría no encontrada para actualizar.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // 1. Manejar la subida de archivos (elimina el anterior si se sube uno nuevo)
                string? nuevaUrl = await GuardarArchivoImagen(imagenFile, categoriaExistente.ImagenUrl);

                // 2. Actualizar las propiedades del objeto existente
                categoriaExistente.Nombre = categoria.Nombre;
                categoriaExistente.ImagenUrl = nuevaUrl; // Asignar la nueva (o misma) URL

                // 3. Guardar en la BD
                await _categoriaRepository.UpdateAsync(categoriaExistente);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar categoría: " + ex.Message;
                return View(categoria); // Devuelve la categoría enviada por el formulario
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!EsAdministrador())
                return RedirectToAction("Login", "Auth");

            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria != null && !string.IsNullOrEmpty(categoria.ImagenUrl))
                {
                    // Lógica para eliminar el archivo físico asociado antes de eliminar la BD
                    string fullPathToDelete = Path.Combine(_webHostEnvironment.WebRootPath, categoria.ImagenUrl.TrimStart('/'));
                    if (System.IO.File.Exists(fullPathToDelete))
                    {
                        System.IO.File.Delete(fullPathToDelete);
                    }
                }

                await _categoriaRepository.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar categoría: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}