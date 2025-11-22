using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Para el tipo decimal en la base de datos, aunque este modelo es temporal/sesión

namespace Appetite_App.Models
{
    /// <summary>
    /// Representa un ítem individual dentro del carrito de compras de un cliente,
    /// utilizado generalmente para almacenar el estado temporal en la sesión HTTP.
    /// Contiene la información necesaria para calcular el precio final, incluyendo los decoradores.
    /// </summary>
    public class ItemCarrito
    {
        /// <summary>
        /// Obtiene o establece un identificador único global (GUID) para gestionar el elemento
        /// dentro del carrito de compras. No es la clave primaria de la base de datos de productos.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Obtiene o establece el identificador del producto base al que hace referencia este ítem.
        /// (Clave foránea lógica, aunque no se mapee directamente en la BD).
        /// </summary>
        [Display(Name = "ID Producto Base")]
        public int IdProducto { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del producto base (ej. "Hamburguesa Clásica").
        /// </summary>
        [Required]
        [StringLength(150)]
        [Display(Name = "Nombre")]
        public string NombreProducto { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece la cantidad de unidades solicitadas de este ítem.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        /// <summary>
        /// Obtiene o establece el precio unitario base del producto, antes de aplicar cualquier decorador.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Precio Base")]
        public decimal PrecioBaseUnitario { get; set; }

        /// <summary>
        /// Obtiene o establece el precio total de la línea de ítem, incluyendo el precio base,
        /// todos los decoradores aplicados y multiplicado por la cantidad (<c>(PrecioBase + Decoradores) * Cantidad</c>).
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Precio Total")]
        public decimal PrecioTotal { get; set; }

        /// <summary>
        /// Obtiene o establece la lista de descripciones de los decoradores aplicados a este ítem
        /// (ej. <c>["Queso Extra", "Carne Doble"]</c>).
        /// </summary>
        [Display(Name = "Extras Aplicados")]
        public List<string> DescripcionExtras { get; set; } = new List<string>();

        /// <summary>
        /// Obtiene o establece la descripción completa final del producto después de aplicar todos
        /// los decoradores, ideal para mostrar en el resumen del carrito o la orden.
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Descripción Completa")]
        public string DescripcionCompleta { get; set; } = string.Empty;
    }
}