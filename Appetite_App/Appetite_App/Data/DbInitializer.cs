using Appetite_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Appetite_App.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            // Usamos un scope para obtener los servicios necesarios
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppetiteContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // 1. Asegurar la creación de la base de datos y migraciones
            // CRÍTICO: SOLO APLICAR MIGRACIONES. NO BORRAR LA BASE DE DATOS.
            Console.WriteLine("[DB INIT] Aplicando migraciones a la base de datos...");
            await context.Database.MigrateAsync();

            Console.WriteLine("[DB INIT] Verificando y creando Roles...");

            // 2. Crear Roles
            string[] roleNames = { "Administrador", "Cliente" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                    Console.WriteLine($"[DB INIT] Rol '{roleName}' creado.");
                }
            }

            // 3. Crear Usuario Administrador (si no existe)
            const string adminEmail = "admin2@appetite.com";
            const string adminPassword = "AdminPassword123!";

            Console.WriteLine($"[DB INIT] Verificando usuario Admin: {adminEmail}");

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
                    Console.WriteLine($"[DB INIT] Usuario Administrador ({adminEmail}) creado exitosamente.");
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                    Console.WriteLine($"[DB INIT] Rol 'Administrador' asignado a {adminEmail}.");
                }
                else
                {
                    Console.WriteLine($"[DB INIT ERROR] Falló la creación del Administrador.");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($" - Código: {error.Code}, Descripción: {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"[DB INIT] Usuario Administrador ({adminEmail}) ya existe. Saltando creación.");
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
                    Console.WriteLine($"[DB INIT] Usuario Cliente ({clientEmail}) creado exitosamente.");
                }
                else
                {
                    Console.WriteLine($"[DB INIT ERROR] Falló la creación del Cliente.");
                }
            }
            else
            {
                Console.WriteLine($"[DB INIT] Usuario Cliente ({clientEmail}) ya existe. Saltando creación.");
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
                Console.WriteLine("[DB INIT] Categorías añadidas.");
                await context.SaveChangesAsync();
            }

            // 6. Verificar y Crear PRODUCTOS (Solo si no existen)
            if (!context.Productos.Any())
            {
                // Recuperar IDs de categorías
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
                        ImagenUrl = "/images/default/hamburguesa_clasica.jpg" // Agregué una URL por defecto
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
                Console.WriteLine("[DB INIT] Productos añadidos.");
                await context.SaveChangesAsync();
            }
            Console.WriteLine("[DB INIT] Inicialización de datos completada.");
        }
    }
}
