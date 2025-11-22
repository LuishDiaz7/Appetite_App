using System;
using System.Collections.Generic;

namespace Appetite_App.Patterns.Decorator
{
    /// <summary>
    /// Define la interfaz común para los objetos que pueden tener un costo y una descripción.
    /// Esta es la interfaz principal del componente (<c>Component</c>) en el Patrón Decorator,
    /// permitiendo que tanto el producto base como sus decoradores implementen la misma firma.
    /// </summary>
    public interface IProductoComponente
    {
        /// <summary>
        /// Obtiene la descripción del componente, incluyendo cualquier modificación o extra añadido
        /// por los decoradores sucesivos.
        /// </summary>
        /// <returns>Una cadena de texto con la descripción detallada.</returns>
        string GetDescripcion();

        /// <summary>
        /// Obtiene el precio total del componente, incluyendo el precio base y el costo
        /// acumulado de todos los decoradores aplicados.
        /// </summary>
        /// <returns>El precio total en formato decimal.</returns>
        decimal GetPrecio();
    }
}
