using System;
using System.Collections.Generic;

namespace Appetite_App.Models
{
    // Modelo usado para almacenar cada producto en el carrito de compras (generalmente en la sesión)
    public class ItemCarrito
    {
        // ID único para manejar el elemento dentro del carrito (no el ID del producto)
        public Guid Id { get; set; } = Guid.NewGuid();

        // ID del producto original (FK)
        public int IdProducto { get; set; }

        // Nombre del producto base (ej. "Hamburguesa Clásica")
        public string NombreProducto { get; set; }

        // Cantidad solicitada
        public int Cantidad { get; set; }

        // Precio unitario base (antes de decoradores)
        public decimal PrecioBaseUnitario { get; set; }

        // Precio total del ítem (base + decoradores) * cantidad
        public decimal PrecioTotal { get; set; }

        // Lista de los decoradores aplicados para mostrar en el carrito (ej. ["Queso Extra", "Carne Doble"])
        // Se inicializa para garantizar que nunca sea null, evitando el error.
        public List<string> DescripcionExtras { get; set; } = new List<string>();

        // Descripcion completa generada por el último decorador (ej: Clásica, Carne Doble, Queso Extra)
        public string DescripcionCompleta { get; set; }
    }
}