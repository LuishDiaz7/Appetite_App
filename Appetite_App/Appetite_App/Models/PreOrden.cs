using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appetite_App.Models
{
    /// <summary>
    /// Representa la cabecera de una orden de compra realizada por un cliente.
    /// Esta clase es clave para la implementación de los patrones BUILDER y OBSERVER.
    /// Es el sujeto (<c>Subject</c>) que notifica a los observadores cuando su estado cambia.
    /// </summary>
    public class PreOrden
    {
        /// <summary>
        /// Obtiene o establece el identificador único de la orden.
        /// Es la clave primaria de la tabla.
        /// </summary>
        [Key]
        [Display(Name = "ID Orden")]
        public int IdOrden { get; set; }

        /// <summary>
        /// Obtiene o establece la clave foránea que enlaza la orden con el usuario que la realizó.
        /// </summary>
        [Display(Name = "ID Cliente")]
        public int IdUsuario { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha y hora en que se creó la orden.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de Orden")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        /// <summary>
        /// Obtiene o establece el estado actual de la orden (ej. "Pendiente", "En Preparación", "Enviada", "Completada").
        /// El cambio en esta propiedad idealmente dispara el patrón Observer.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente";

        /// <summary>
        /// Obtiene o establece la dirección de entrega de la orden.
        /// </summary>
        [Required]
        [StringLength(250)]
        [Display(Name = "Dirección de Entrega")]
        public string Direccion { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece el costo total final de la orden (suma de todos los detalles).
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        // Propiedades de navegación

        /// <summary>
        /// Obtiene o establece el objeto <see cref="Usuario"/> que realizó esta orden.
        /// (Relación uno a muchos: un Usuario tiene muchas PreOrdenes).
        /// </summary>
        public Usuario? Usuario { get; set; }

        /// <summary>
        /// Obtiene o establece la colección de <see cref="DetalleOrden"/> asociados a esta orden.
        /// (Relación uno a muchos: una PreOrden tiene muchos Detalles).
        /// </summary>
        public ICollection<DetalleOrden> Detalles { get; set; } = new List<DetalleOrden>();
    }
}