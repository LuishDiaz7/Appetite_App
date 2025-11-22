using Microsoft.EntityFrameworkCore;
using Appetite_App.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Data
{
    /// <summary>
    /// Contexto de base de datos para la aplicación Appetite, utilizando Entity Framework Core (EF Core).
    /// Hereda de <c>IdentityDbContext</c> para manejar las tablas de usuarios y roles
    /// de ASP.NET Core Identity, utilizando <see cref="Usuario"/> como clase de usuario y
    /// <c>int</c> como tipo de clave primaria para los usuarios y roles.
    /// 
    /// </summary>
    public class AppetiteContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
    {
        /// <summary>
        /// Inicializa una nueva instancia del <see cref="AppetiteContext"/>.
        /// </summary>
        /// <param name="options">Opciones de configuración del contexto, generalmente inyectadas por DI.</param>
        public AppetiteContext(DbContextOptions<AppetiteContext> options)
          : base(options)
        {
        }

        // DbSets - Mapeo de Modelos a Tablas de la DB

        /// <summary>
        /// Colección que representa la tabla de Productos.
        /// </summary>
        public DbSet<Producto> Productos { get; set; } = default!;

        /// <summary>
        /// Colección que representa la tabla de Categorías.
        /// </summary>
        public DbSet<Categoria> Categorias { get; set; } = default!;

        /// <summary>
        /// Colección que representa la tabla de Órdenes (la cabecera de la orden).
        /// </summary>
        public DbSet<PreOrden> Ordenes { get; set; } = default!;

        /// <summary>
        /// Colección que representa la tabla de los Detalles de las Órdenes (ítems).
        /// </summary>
        public DbSet<DetalleOrden> DetallesOrdenes { get; set; } = default!;

        /// <summary>
        /// Colección que representa la tabla de Usuarios. Aunque ya está en la base, 
        /// se incluye para facilitar las consultas LINQ.
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; } = default!;

        /// <summary>
        /// Sobrescribe este método para configurar el esquema de la base de datos,
        /// incluyendo claves primarias, claves compuestas y tipos de columna.
        /// </summary>
        /// <param name="modelBuilder">El constructor de modelos utilizado para configurar las entidades.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // **Paso CRÍTICO: Debe llamar a la implementación base para que Identity configure sus tablas.**
            base.OnModelCreating(modelBuilder);

            // Renombrar la tabla de usuarios de "AspNetUsers" a "Usuarios" (opcional pero más claro)
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");

            // 1. Configuración de CLAVE PRIMARIA para Categoria (si no sigue la convención)
            modelBuilder.Entity<Categoria>()
                .HasKey(c => c.IdCategoria);

            // 2. Configuración de CLAVE COMPUESTA para DetalleOrden
            modelBuilder.Entity<DetalleOrden>()
                .HasKey(do_ => new { do_.IdOrden, do_.IdProducto });

            // 3. Definición explícita de relaciones para DetalleOrden (Relación M:N a través de esta tabla)

            // Relación N:1 de DetalleOrden a PreOrden
            modelBuilder.Entity<DetalleOrden>()
                .HasOne(do_ => do_.PreOrden)
                .WithMany(o => o.Detalles) // Una PreOrden tiene muchos DetallesOrden
                .HasForeignKey(do_ => do_.IdOrden);

            // Relación N:1 de DetalleOrden a Producto
            modelBuilder.Entity<DetalleOrden>()
                .HasOne(do_ => do_.Producto)
                .WithMany(p => p.DetallesOrden) // Un Producto puede estar en muchos DetallesOrden
                .HasForeignKey(do_ => do_.IdProducto);

            // 4. Configuración de precisión para tipos decimales (Crucial para evitar errores de redondeo en dinero)
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<DetalleOrden>()
                .Property(d => d.PrecioUnitario)
                .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<DetalleOrden>()
                .Property(d => d.Subtotal)
                .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<PreOrden>()
                .Property(p => p.Total)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
