using Microsoft.AspNetCore.Mvc;
using Appetite_App.Models;
using Appetite_App.Repositories;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization; // NECESARIO para [Authorize]

namespace Appetite.Controllers
{
    // 1. AUTORIZACIÓN: Solo usuarios con el rol "Administrador" pueden acceder.
    [Authorize(Roles = "Administrador")]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoriaController(ICategoriaRepository categoriaRepository, IWebHostEnvironment webHostEnvironment)
        {
            _categoriaRepository = categoriaRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // Lógica central para guardar el archivo (se mantiene sin cambios)
        private async Task<string?> GuardarArchivoImagen(IFormFile? imagenFile, string? existingImagePath)
        {
            if (imagenFile == null)
            {
                return existingImagePath;
            }

            // 1. Eliminar la imagen anterior si existe (para evitar archivos huérfanos)
            if (!string.IsNullOrEmpty(existingImagePath))
            {
                string fullPathToDelete = Path.Combine(_webHostEnvironment.WebRootPath, existingImagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPathToDelete))
                {
                    try { System.IO.File.Delete(fullPathToDelete); } catch { /* Ignorar errores de eliminación */ }
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
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imagenFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imagenFile.CopyToAsync(fileStream);
            }

            // 4. Devolver la ruta relativa a wwwroot
            return $"/images/categorias/{uniqueFileName}";
        }


        // =========================================================================================
        // ACCIÓN INDEX (LISTADO)
        // =========================================================================================
        public async Task<IActionResult> Index()
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            // 🚨 CORRECCIÓN RUTA DE VISTA: Apuntar explícitamente a la vista dentro de Admin.
            return View("~/Views/Categoria/Index.cshtml", categorias);
        }

        // =========================================================================================
        // ACCIÓN CREAR GET
        // =========================================================================================
        [HttpGet]
        public IActionResult Crear()
        {
            // 🚨 CORRECCIÓN RUTA DE VISTA: Apuntar explícitamente a la vista dentro de Admin.
            return View("~/Views/Categoria/Crear.cshtml");
        }

        // =========================================================================================
        // ACCIÓN CREAR POST
        // =========================================================================================
        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria, IFormFile? imagenFile)
        {
            // NOTA: Se ha eliminado la verificación manual de EsAdministrador()

            try
            {
                categoria.ImagenUrl = await GuardarArchivoImagen(imagenFile, null);

                // Advertencia CS8601 potencial si el modelo no está bien configurado:
                // Asegúrate de que las propiedades del modelo (Nombre, etc.) no sean nulas si son non-nullable.

                await _categoriaRepository.AddAsync(categoria);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al crear categoría: " + ex.Message;
                // 🚨 CORRECCIÓN RUTA DE VISTA: Devolver la vista CrearCategoria con el modelo si hay error.
                return View("~/Views/Categoria/Crear.cshtml", categoria);
            }
        }

        // =========================================================================================
        // ACCIÓN EDITAR GET
        // =========================================================================================
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
            {
                TempData["Error"] = "Categoría no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            // 🚨 CORRECCIÓN RUTA DE VISTA: Apuntar a la vista de edición correcta.
            // (Asumo que la tienes como 'EditarCategoria.cshtml' o similar en /Admin)
            return View("~/Views/Categoria/Editar.cshtml", categoria);
        }

        // =========================================================================================
        // ACCIÓN EDITAR POST
        // =========================================================================================
        [HttpPost]
        public async Task<IActionResult> Editar(int id, Categoria categoria, IFormFile? imagenFile)
        {
            if (id != categoria.IdCategoria)
            {
                TempData["Error"] = "ID de Categoría no coincide.";
                return RedirectToAction(nameof(Index));
            }

            var categoriaExistente = await _categoriaRepository.GetByIdAsync(id);
            if (categoriaExistente == null)
            {
                TempData["Error"] = "Categoría no encontrada para actualizar.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Guarda el nuevo archivo y elimina el anterior. Retorna la nueva URL o la existente.
                string? nuevaUrl = await GuardarArchivoImagen(imagenFile, categoriaExistente.ImagenUrl);

                categoriaExistente.Nombre = categoria.Nombre;
                // 🔑 CRÍTICO: Aseguramos que la URL se actualice en el modelo que se va a guardar.
                categoriaExistente.ImagenUrl = nuevaUrl;

                await _categoriaRepository.UpdateAsync(categoriaExistente);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar categoría: " + ex.Message;
                // 🚨 CORRECCIÓN RUTA DE VISTA: Devolver la vista EditarCategoria con el modelo si hay error.
                return View("~/Views/Categoria/Editar.cshtml", categoria);
            }
        }

        // =========================================================================================
        // ACCIÓN ELIMINAR POST
        // =========================================================================================
        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria != null && !string.IsNullOrEmpty(categoria.ImagenUrl))
                {
                    // Lógica para eliminar el archivo físico asociado
                    string fullPathToDelete = Path.Combine(_webHostEnvironment.WebRootPath, categoria.ImagenUrl.TrimStart('/'));
                    if (System.IO.File.Exists(fullPathToDelete))
                    {
                        try { System.IO.File.Delete(fullPathToDelete); } catch { /* Ignorar errores de eliminación */ }
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