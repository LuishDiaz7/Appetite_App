using Appetite_App.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Appetite_App.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(AppetiteContext context)
        {
            // Asegúrate de que la base de datos esté creada (aunque ya la hiciste con Update-Database)
            await context.Database.MigrateAsync();

            // 1. Verificar y Crear USUARIOS
            if (!context.Usuarios.Any())
            {
                // La clave primaria para Usuario es IdUsuario, y EF Core la gestiona.
                // Usamos un Hash simple para el administrador (en una app real usarías BCrypt, etc.)
                string adminPasswordHash = "511f9a18f1103169cbb75be603c4f3babfe85eeedd7c12f5dd77c9b95cc9ae3f"; // Simulación

                var usuarios = new Usuario[]
                {
                    new Usuario {
                        Nombre = "Admin Global",
                        Email = "admin@appetite.com",
                        PasswordHash = adminPasswordHash,
                        Rol = "Administrador"
                    },
                    new Usuario {
                        Nombre = "Cliente Prueba",
                        Email = "cliente@appetite.com",
                        PasswordHash = adminPasswordHash,
                        Rol = "Cliente"
                    }
                };

                await context.Usuarios.AddRangeAsync(usuarios);
                await context.SaveChangesAsync();
            }

            // 2. Verificar y Crear CATEGORÍAS
            if (!context.Categorias.Any())
            {
                var categorias = new Categoria[]
                {
                    new Categoria { Nombre = "Hamburguesas" },
                    new Categoria { Nombre = "Bebidas" },
                    new Categoria { Nombre = "Postres" }
                };

                await context.Categorias.AddRangeAsync(categorias);
                await context.SaveChangesAsync();
            }

            // 3. Verificar y Crear PRODUCTOS
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
                        IdCategoria = categoriaHamb
                    },
                    new Producto {
                        Nombre = "Vegetariana",
                        Descripcion = "Hecha con lentejas y pan integral.",
                        Precio = 8.00m,
                        Stock = 30,
                        IdCategoria = categoriaHamb
                    },
                    new Producto {
                        Nombre = "Gaseosa Personal",
                        Descripcion = "Bebida azucarada con hielo.",
                        Precio = 2.50m,
                        Stock = 100,
                        IdCategoria = categoriaBebida
                    }
                };

                await context.Productos.AddRangeAsync(productos);
                await context.SaveChangesAsync();
            }
        }
    }
}
