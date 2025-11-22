using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Appetite_App.Models
{
    /// <summary>
    /// Representa un producto o ítem ofrecido en el menú de la aplicación.
    /// Esta clase sirve como el <c>ConcreteComponent</c> (Componente Concreto) inicial
    /// para la implementación del Patrón Decorator.
    /// </summary>
    public class Producto
    {
        /// <summary>
        /// Obtiene o establece el identificador único del producto.
        /// Es la clave primaria de la tabla Producto.
        /// </summary>
        [Key]
        public int IdProducto { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del producto (ej. "Hamburguesa Clásica").
        /// </summary>
        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece una descripción detallada del producto.
        /// </summary>
        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece el precio unitario base del producto.
        /// </summary>
        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, 10000.00, ErrorMessage = "El precio debe ser positivo.")]
        public decimal Precio { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad disponible de este producto en inventario.
        /// </summary>
        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si el producto está disponible para la venta.
        /// Por defecto es <c>true</c>.
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Obtiene o establece la URL relativa donde se almacena la imagen del producto.
        /// Puede ser nulo.
        /// </summary>
        [Display(Name = "URL de Imagen")]
        [StringLength(250)]
        public string? ImagenUrl { get; set; }

        // Clave Foránea

        /// <summary>
        /// Obtiene o establece la clave foránea que enlaza el producto con su categoría.
        /// </summary>
        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public int IdCategoria { get; set; }

        // Propiedad de navegación

        /// <summary>
        /// Obtiene o establece la categoría a la que pertenece este producto.
        /// (Relación uno a muchos: una Categoría tiene muchos Productos).
        /// </summary>
        [ForeignKey("IdCategoria")]
        public Categoria? Categoria { get; set; }

        /// <summary>
        /// Obtiene o establece la colección de detalles de órdenes en los que aparece este producto.
        /// (Relación uno a muchos: un Producto puede aparecer en muchos Detalles de Orden).
        /// </summary>
        public ICollection<DetalleOrden> DetallesOrden { get; set; } = new List<DetalleOrden>();
    }
}