using Microsoft.EntityFrameworkCore;
using Appetite_App.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Appetite_App.Data
{
    // CAMBIO CRÍTICO: Heredar de IdentityDbContext.
    // Los argumentos son: 
    // 1. La clase de Usuario (Usuario)
    // 2. La clase de Rol (IdentityRole<int>)
    // 3. El tipo de la clave de usuario/rol (int)
    public class AppetiteContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
    {
        public AppetiteContext(DbContextOptions<AppetiteContext> options)
          : base(options)
        {
        }

        // DbSets - Mapeo de Modelos a Tablas de la DB
        // Nota: Los DbSets para Usuario y Role son manejados internamente por IdentityDbContext.
        public DbSet<Producto> Productos { get; set; } = default!;
        public DbSet<Categoria> Categorias { get; set; } = default!;
        public DbSet<PreOrden> Ordenes { get; set; } = default!;
        public DbSet<DetalleOrden> DetallesOrdenes { get; set; } = default!;

        public DbSet<Usuario> Usuarios { get; set; } = default!;

        // Sobrescribimos OnModelCreating para configurar claves que no siguen la convención
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // **Paso CRÍTICO: Debe llamar a la implementación base para que Identity configure sus tablas.**
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("Usuarios");

            // 1. Configuración de CLAVE PRIMARIA para Categoria
            modelBuilder.Entity<Categoria>()
        .HasKey(c => c.IdCategoria);

            // 2. Configuración de CLAVE COMPUESTA para DetalleOrden
            modelBuilder.Entity<DetalleOrden>()
        .HasKey(do_ => new { do_.IdOrden, do_.IdProducto });

            // 3. Definición explícita de relaciones para DetalleOrden
            modelBuilder.Entity<DetalleOrden>()
        .HasOne(do_ => do_.PreOrden)
        .WithMany(o => o.Detalles)
        .HasForeignKey(do_ => do_.IdOrden);

            // Relación con Producto
            modelBuilder.Entity<DetalleOrden>()
        .HasOne(do_ => do_.Producto)
        .WithMany(p => p.DetallesOrden)
        .HasForeignKey(do_ => do_.IdProducto);

            // Opcional: Para evitar los warnings de 'decimal' que salieron en la consola
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
