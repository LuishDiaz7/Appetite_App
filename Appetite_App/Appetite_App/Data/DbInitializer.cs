using Appetite_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Data
{
    /// <summary>
    /// Clase estática responsable de inicializar la base de datos de la aplicación.
    /// Realiza la aplicación de migraciones, la creación de roles, y la siembra (seeding) de datos
    /// esenciales como usuarios administradores, clientes de prueba y categorías de productos.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Método principal de inicialización que orquesta la configuración de la base de datos.
        /// Se llama durante el arranque de la aplicación (<c>Program.cs</c>).
        /// </summary>
        /// <param name="serviceProvider">El proveedor de servicios del contenedor de Inyección de Dependencias.</param>
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            // Se utiliza un scope para garantizar que los servicios obtenidos sean desechados correctamente
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppetiteContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // CORRECCIÓN CS0718: Se cambió DbInitializer por AppetiteContext como argumento de tipo
            // ya que los tipos estáticos no pueden ser usados en genéricos.
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppetiteContext>>();

            // 1. Asegurar la creación de la base de datos y migraciones
            // CRÍTICO: SOLO APLICAR MIGRACIONES. NO BORRAR LA BASE DE DATOS.
            logger.LogInformation("[DB INIT] Aplicando migraciones a la base de datos...");
            await context.Database.MigrateAsync();

            logger.LogInformation("[DB INIT] Verificando y creando Roles...");

            // 2. Crear Roles
            string[] roleNames = { "Administrador", "Cliente" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                    logger.LogInformation($"[DB INIT] Rol '{roleName}' creado.");
                }
            }

            // 3. Crear Usuario Administrador (si no existe)
            const string adminEmail = "admin2@appetite.com";
            const string adminPassword = "AdminPassword123!";

            logger.LogInformation($"[DB INIT] Verificando usuario Admin: {adminEmail}");

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new Usuario
                {
                    Nombre = "Admin Global",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    logger.LogInformation($"[DB INIT] Usuario Administrador ({adminEmail}) creado exitosamente.");
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                    logger.LogInformation($"[DB INIT] Rol 'Administrador' asignado a {adminEmail}.");
                }
                else
                {
                    logger.LogError($"[DB INIT ERROR] Falló la creación del Administrador.");
                    foreach (var error in result.Errors)
                    {
                        logger.LogError($" - Código: {error.Code}, Descripción: {error.Description}");
                    }
                }
            }
            else
            {
                logger.LogInformation($"[DB INIT] Usuario Administrador ({adminEmail}) ya existe. Saltando creación.");
            }

            // 4. Crear Usuario Cliente de Prueba (si no existe)
            const string clientEmail = "cliente@appetite.com";
            const string clientPassword = "ClientPassword123!";

            if (await userManager.FindByEmailAsync(clientEmail) == null)
            {
                var clientUser = new Usuario
                {
                    Nombre = "Cliente Prueba",
                    UserName = clientEmail,
                    Email = clientEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(clientUser, clientPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(clientUser, "Cliente");
                    logger.LogInformation($"[DB INIT] Usuario Cliente ({clientEmail}) creado exitosamente.");
                }
                else
                {
                    logger.LogError($"[DB INIT ERROR] Falló la creación del Cliente.");
                }
            }
            else
            {
                logger.LogInformation($"[DB INIT] Usuario Cliente ({clientEmail}) ya existe. Saltando creación.");
            }

            // 5. Verificar y Crear CATEGORÍAS (Solo si no existen)
            if (!context.Categorias.Any())
            {
                var categorias = new Categoria[]
                {
                    new Categoria { Nombre = "Hamburguesas" },
                    new Categoria { Nombre = "Bebidas" },
                    new Categoria { Nombre = "Postres" }
                };

                await context.Categorias.AddRangeAsync(categorias);
                logger.LogInformation("[DB INIT] Categorías añadidas.");
                await context.SaveChangesAsync();
            }

            // 6. Verificar y Crear PRODUCTOS (Solo si no existen)
            if (!context.Productos.Any())
            {
                // Recuperar IDs de categorías (Asumimos que las categorías existen en este punto)
                var categoriaHamb = context.Categorias.First(c => c.Nombre == "Hamburguesas").IdCategoria;
                var categoriaBebida = context.Categorias.First(c => c.Nombre == "Bebidas").IdCategoria;

                var productos = new Producto[]
                {
                    new Producto {
                        Nombre = "Clásica",
                        Descripcion = "Doble carne, doble queso, salsa especial.",
                        Precio = 10.50m,
                        Stock = 50,
                        Activo = true,
                        IdCategoria = categoriaHamb,
                        ImagenUrl = "/images/default/hamburguesa_clasica.jpg" // URL por defecto
                    },
                    new Producto {
                        Nombre = "Vegetariana",
                        Descripcion = "Hecha con lentejas y pan integral.",
                        Precio = 8.00m,
                        Stock = 30,
                        Activo = true,
                        IdCategoria = categoriaHamb,
                        ImagenUrl = "/images/default/hamburguesa_vegetariana.jpg"
                    },
                    new Producto {
                        Nombre = "Gaseosa Personal",
                        Descripcion = "Bebida azucarada con hielo.",
                        Precio = 2.50m,
                        Stock = 100,
                        Activo = true,
                        IdCategoria = categoriaBebida,
                        ImagenUrl = "/images/default/gaseosa_personal.jpg"
                    }
                };

                await context.Productos.AddRangeAsync(productos);
                logger.LogInformation("[DB INIT] Productos añadidos.");
                await context.SaveChangesAsync();
            }
            logger.LogInformation("[DB INIT] Inicialización de datos completada.");
        }
    }
}