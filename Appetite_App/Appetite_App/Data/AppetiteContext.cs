using Microsoft.EntityFrameworkCore;
using Appetite_App.Models;
using System.Security.Cryptography.X509Certificates;

namespace Appetite_App.Data
{
    public class AppetiteContext : DbContext
    {
        public AppetiteContext(DbContextOptions<AppetiteContext> options)
            : base(options)
        {
        }

        // DbSets - Mapeo de Modelos a Tablas de la DB
        public DbSet<Usuario> Usuarios { get; set; } = default!;
        public DbSet<Producto> Productos { get; set; } = default!;
        public DbSet<Categoria> Categorias { get; set; } = default!;
        public DbSet<PreOrden> Ordenes { get; set; } = default!;
        public DbSet<DetalleOrden> DetallesOrdenes { get; set; } = default!;

        // Sobrescribimos OnModelCreating para configurar claves que no siguen la convención
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Configuración de CLAVE PRIMARIA para Categoria (Resuelve el primer error)
            // Esto es necesario porque usamos 'IdCategoria' en lugar de 'CategoriaId'.
            modelBuilder.Entity<Categoria>()
                .HasKey(c => c.IdCategoria);

            // 2. Configuración de CLAVE COMPUESTA para DetalleOrden (Resuelve el segundo error)
            // La clave compuesta es la combinación de las dos claves foráneas.
            modelBuilder.Entity<DetalleOrden>()
                .HasKey(do_ => new { do_.IdOrden, do_.IdProducto });

            // 3. Definición explícita de relaciones para DetalleOrden (Buenas prácticas)
            // Relación con PreOrden
            modelBuilder.Entity<DetalleOrden>()
                .HasOne(do_ => do_.PreOrden)
                .WithMany(o => o.Detalles)
                .HasForeignKey(do_ => do_.IdOrden);

            // Relación con Producto
            modelBuilder.Entity<DetalleOrden>()
                .HasOne(do_ => do_.Producto)
                .WithMany(p => p.DetallesOrden)
                .HasForeignKey(do_ => do_.IdProducto);

            // Asegúrate de llamar a la implementación base si estás configurando otros aspectos
            base.OnModelCreating(modelBuilder);
        }
    }
}
