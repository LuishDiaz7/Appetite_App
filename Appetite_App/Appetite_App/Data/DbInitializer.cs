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

            // CRÍTICO: Eliminar la base de datos anterior para asegurar el nuevo esquema de Identity
            Console.WriteLine("[DB INIT] Eliminando base de datos existente...");
            await context.Database.EnsureDeletedAsync();

            // 1. Asegurar la creación de la base de datos y migraciones
            Console.WriteLine("[DB INIT] Creando la base de datos y aplicando migraciones...");
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
            // CAMBIO CRÍTICO: Usaremos un nuevo email para forzar la recreación del usuario
            const string adminEmail = "admin2@appetite.com"; // <<<<< NUEVO EMAIL
            const string adminPassword = "AdminPassword123!"; // <<< MISMA CONTRASEÑA

            Console.WriteLine($"[DB INIT] Verificando usuario Admin: {adminEmail}");

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new Usuario
                {
                    Nombre = "Admin Global",
                    // Asegúrese de que UserName y Email sean el nuevo email
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
                    // Esto es clave: imprime los errores si la creación falla (ej. requerimientos de contraseña)
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

            // --- El resto de su inicializador (Cliente, Categorías, Productos) se mantiene igual ---

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

            // 5. Verificar y Crear CATEGORÍAS
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

            // 6. Verificar y Crear PRODUCTOS
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
                        IdCategoria = categoriaHamb
                    },
                    new Producto {
                        Nombre = "Vegetariana",
                        Descripcion = "Hecha con lentejas y pan integral.",
                        Precio = 8.00m,
                        Stock = 30,
                        Activo = true,
                        IdCategoria = categoriaHamb
                    },
                    new Producto {
                        Nombre = "Gaseosa Personal",
                        Descripcion = "Bebida azucarada con hielo.",
                        Precio = 2.50m,
                        Stock = 100,
                        Activo = true,
                        IdCategoria = categoriaBebida
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
