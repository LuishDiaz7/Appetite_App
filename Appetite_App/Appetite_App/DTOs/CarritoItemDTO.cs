using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.DTOs
{
    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) utilizado para transmitir la información
    /// de un ítem individual desde el cliente (carrito de compras) a la capa de servicio.
    /// Contiene la información necesaria para que el Patrón Decorator pueda reconstruir
    /// el producto modificado.
    /// </summary>
    public class CarritoItemDTO
    {
        /// <summary>
        /// Obtiene o establece el identificador del producto base al que se aplica esta configuración.
        /// Corresponde al <c>IdProducto</c> del modelo de dominio.
        /// </summary>
        [Required]
        [Display(Name = "ID Producto Base")]
        public int IdProducto { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad de unidades solicitadas de este ítem.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        /// <summary>
        /// Obtiene o establece la lista de nombres de los decoradores a aplicar al producto.
        /// Estos nombres se utilizan en el <see cref="Services.OrdenService"/> para instanciar
        /// los <c>ConcreteDecorator</c>s (ej: "QuesoExtra", "CarneDoble").
        /// Se inicializa para garantizar que la lista nunca sea nula.
        /// </summary>
        public List<string> Decoradores { get; set; } = new List<string>();
    }
}
