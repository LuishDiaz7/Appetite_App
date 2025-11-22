using Microsoft.AspNetCore.Mvc;
using Appetite_App.Models;
using Appetite_App.Repositories;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace Appetite_App.Controllers
{
    /// <summary>
    /// Controlador encargado de la gestión de categorías de productos, incluyendo
    /// la creación, lectura, actualización y eliminación (CRUD).
    /// Requiere que el usuario esté autenticado con el rol de Administrador.
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="CategoriaController"/>.
        /// </summary>
        /// <param name="categoriaRepository">El repositorio para acceder a la capa de persistencia de categorías.</param>
        /// <param name="webHostEnvironment">Proporciona información sobre el entorno de alojamiento web (ej. ruta raíz).</param>
        public CategoriaController(ICategoriaRepository categoriaRepository, IWebHostEnvironment webHostEnvironment)
        {
            _categoriaRepository = categoriaRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // ---------------------------------------------
        // MÉTODOS DE MANEJO DE ARCHIVOS (PRIVADOS)
        // ---------------------------------------------

        /// <summary>
        /// Guarda un archivo de imagen en el sistema de archivos del servidor y retorna la ruta relativa.
        /// También maneja la eliminación de una imagen anterior si se está actualizando.
        /// </summary>
        /// <param name="imagenFile">El nuevo archivo de imagen subido por el usuario. Puede ser nulo.</param>
        /// <param name="existingImagePath">La ruta relativa de la imagen existente (anterior) para su posible eliminación.</param>
        /// <returns>La nueva URL relativa de la imagen guardada, o la ruta existente si no se subió una nueva imagen.</returns>
        private async Task<string?> GuardarArchivoImagen(IFormFile? imagenFile, string? existingImagePath)
        {
            // Si no se sube un nuevo archivo, se mantiene la ruta existente
            if (imagenFile == null)
            {
                return existingImagePath;
            }

            // 1. Eliminar la imagen anterior si existe
            if (!string.IsNullOrEmpty(existingImagePath))
            {
                // La ruta se ajusta para el sistema de archivos (quitar '/' inicial)
                string fullPathToDelete = Path.Combine(_webHostEnvironment.WebRootPath, existingImagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPathToDelete))
                {
                    try { System.IO.File.Delete(fullPathToDelete); } catch { /* Se ignora el error de eliminación para no bloquear el proceso de guardado */ }
                }
            }

            // 2. Definir la carpeta de subidas y asegurar su existencia
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "categorias");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 3. Generar un nombre único y guardar el nuevo archivo
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imagenFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imagenFile.CopyToAsync(fileStream);
            }

            // 4. Devolver la ruta relativa (útil para uso en la vista)
            return $"/images/categorias/{uniqueFileName}";
        }


        // ---------------------------------------------
        // ACCIÓN INDEX (LISTADO)
        // ---------------------------------------------

        /// <summary>
        /// Muestra la lista de todas las categorías de productos registradas.
        /// </summary>
        /// <returns>Una vista que contiene una colección de objetos <see cref="Categoria"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Categoria> categorias = await _categoriaRepository.GetAllAsync();
            // Utiliza la ruta completa de la vista si está fuera del contexto del controlador
            return View("~/Views/Categoria/Index.cshtml", categorias);
        }

        // ---------------------------------------------
        // ACCIONES CREAR (GET/POST)
        // ---------------------------------------------

        /// <summary>
        /// Muestra el formulario para crear una nueva categoría.
        /// </summary>
        /// <returns>La vista del formulario de creación.</returns>
        [HttpGet]
        public IActionResult Crear()
        {
            return View("~/Views/Categoria/Crear.cshtml");
        }

        /// <summary>
        /// Procesa los datos enviados para crear una nueva categoría, incluyendo la subida de una imagen.
        /// </summary>
        /// <param name="categoria">El objeto <see cref="Categoria"/> con los datos del formulario.</param>
        /// <param name="imagenFile">El archivo de imagen subido por el usuario.</param>
        /// <returns>Redirecciona a la acción <see cref="Index"/> si la creación es exitosa; de lo contrario, regresa a la vista de creación con el modelo.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Categoria categoria, IFormFile? imagenFile)
        {
            try
            {
                // Guarda la imagen y actualiza la URL en el modelo
                categoria.ImagenUrl = await GuardarArchivoImagen(imagenFile, null);

                await _categoriaRepository.AddAsync(categoria);
                TempData["Success"] = "Categoría creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al crear categoría: {ex.Message}";
                return View("~/Views/Categoria/Crear.cshtml", categoria);
            }
        }

        // ---------------------------------------------
        // ACCIONES EDITAR (GET/POST)
        // ---------------------------------------------

        /// <summary>
        /// Muestra el formulario para editar una categoría existente.
        /// </summary>
        /// <param name="id">El identificador único de la categoría a editar.</param>
        /// <returns>La vista del formulario de edición con la categoría precargada o una redirección si no se encuentra.</returns>
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            Categoria? categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
            {
                TempData["Error"] = "Categoría no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Categoria/Editar.cshtml", categoria);
        }

        /// <summary>
        /// Procesa los datos enviados para actualizar una categoría existente, incluyendo la posible sustitución de su imagen.
        /// </summary>
        /// <param name="id">El identificador único de la categoría a editar (de la ruta).</param>
        /// <param name="categoria">El objeto <see cref="Categoria"/> con los datos actualizados del formulario.</param>
        /// <param name="imagenFile">El nuevo archivo de imagen subido (opcional).</param>
        /// <returns>Redirecciona a la acción <see cref="Index"/> si la actualización es exitosa; de lo contrario, regresa a la vista de edición con errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Categoria categoria, IFormFile? imagenFile)
        {
            if (id != categoria.IdCategoria)
            {
                TempData["Error"] = "ID de Categoría no coincide.";
                return RedirectToAction(nameof(Index));
            }

            Categoria? categoriaExistente = await _categoriaRepository.GetByIdAsync(id);
            if (categoriaExistente == null)
            {
                TempData["Error"] = "Categoría no encontrada para actualizar.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Guarda la nueva imagen (o mantiene la anterior y elimina la vieja si se subió una nueva)
                string? nuevaUrl = await GuardarArchivoImagen(imagenFile, categoriaExistente.ImagenUrl);

                // Actualizar las propiedades del modelo existente
                categoriaExistente.Nombre = categoria.Nombre;
                categoriaExistente.ImagenUrl = nuevaUrl;

                await _categoriaRepository.UpdateAsync(categoriaExistente);
                TempData["Success"] = "Categoría actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al actualizar categoría: {ex.Message}";
                return View("~/Views/Categoria/Editar.cshtml", categoria);
            }
        }

        // ---------------------------------------------
        // ACCIÓN ELIMINAR (POST)
        // ---------------------------------------------

        /// <summary>
        /// Elimina una categoría del sistema y el archivo de imagen físico asociado.
        /// </summary>
        /// <param name="id">El identificador único de la categoría a eliminar.</param>
        /// <returns>Redirecciona a la acción <see cref="Index"/> tras la eliminación.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                Categoria? categoria = await _categoriaRepository.GetByIdAsync(id);

                // 1. Eliminar archivo físico si existe
                if (categoria != null && !string.IsNullOrEmpty(categoria.ImagenUrl))
                {
                    string fullPathToDelete = Path.Combine(_webHostEnvironment.WebRootPath, categoria.ImagenUrl.TrimStart('/'));
                    if (System.IO.File.Exists(fullPathToDelete))
                    {
                        try { System.IO.File.Delete(fullPathToDelete); } catch { /* Ignorar */ }
                    }
                }

                // 2. Eliminar registro de la base de datos
                await _categoriaRepository.DeleteAsync(id);
                TempData["Success"] = "Categoría eliminada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar categoría: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}