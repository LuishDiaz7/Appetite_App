using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Appetite_App.Models
{
    /// <summary>
    /// Representa una categoría de productos en el sistema (ej. "Hamburguesas", "Bebidas", "Postres").
    /// Esta clase es parte del modelo de dominio y se mapea a una tabla en la base de datos.
    /// </summary>
    public class Categoria
    {
        /// <summary>
        /// Obtiene o establece el identificador único de la categoría.
        /// Es la clave primaria de la tabla.
        /// </summary>
        [Key]
        public int IdCategoria { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la categoría.
        /// No debe ser nulo.
        /// </summary>
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece la URL relativa donde se almacena la imagen representativa de la categoría.
        /// Puede ser nulo si no tiene imagen.
        /// </summary>
        [Display(Name = "URL de Imagen")]
        public string? ImagenUrl { get; set; }

        // Propiedad de navegación

        /// <summary>
        /// Obtiene o establece la colección de productos asociados a esta categoría.
        /// Esta es una propiedad de navegación que representa la relación uno a muchos (una Categoría tiene muchos Productos).
        /// </summary>
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
