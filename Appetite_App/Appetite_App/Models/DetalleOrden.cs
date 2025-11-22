using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appetite_App.Models
{
    /// <summary>
    /// Representa una línea de ítem dentro de una orden de compra (<see cref="PreOrden"/>).
    /// Almacena los detalles específicos de un producto en particular, incluyendo la cantidad,
    /// el precio final unitario (tras aplicar descuentos/aumentos) y los decoradores utilizados.
    /// </summary>
    public class DetalleOrden
    {
        /// <summary>
        /// Obtiene o establece el identificador único del detalle de la orden.
        /// Es la clave primaria de la tabla DetalleOrden.
        /// </summary>
        [Key]
        public int IdDetalle { get; set; }

        /// <summary>
        /// Obtiene o establece la clave foránea que enlaza este detalle con la orden principal.
        /// </summary>
        public int IdOrden { get; set; }

        /// <summary>
        /// Obtiene o establece la clave foránea que enlaza este detalle con el producto base.
        /// </summary>
        public int IdProducto { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad de este producto ordenado.
        /// </summary>
        public int Cantidad { get; set; }

        /// <summary>
        /// Obtiene o establece el precio unitario del ítem **incluyendo los costos de los decoradores aplicados**
        /// en el momento de la compra.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecioUnitario { get; set; }

        /// <summary>
        /// Obtiene o establece el subtotal de la línea de ítem (PrecioUnitario * Cantidad).
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Obtiene o establece una cadena de texto que describe los decoradores (modificadores)
        /// aplicados al producto en esta orden (ej: "Queso Extra, Carne Doble").
        /// Esto es crucial para mantener la lógica del Patrón Decorator persistida.
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Decoradores Aplicados")]
        public string DecoradoresAplicados { get; set; } = string.Empty;

        // Propiedades de navegación

        /// <summary>
        /// Obtiene o establece la orden principal a la que pertenece este detalle.
        /// </summary>
        public PreOrden? Orden { get; set; }

        /// <summary>
        /// Obtiene o establece el producto base al que hace referencia este detalle.
        /// </summary>
        public Producto? Producto { get; set; }

        /// <summary>
        /// Obtiene o establece la orden principal (otra forma de definir la FK).
        /// Se inicializa como 'default!' para satisfacer el compilador si se espera que no sea nulo.
        /// </summary>
        // Nota: A menudo se recomienda usar solo una propiedad de navegación hacia la principal (Orden), 
        // pero se mantiene PreOrden si el framework lo requiere o por convención de la aplicación.
        public PreOrden PreOrden { get; set; } = default!;
    }
}
