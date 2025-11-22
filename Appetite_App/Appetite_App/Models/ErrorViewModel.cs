using System;
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Models
{
    /// <summary>
    /// ViewModel utilizado para pasar información de manejo de errores a las vistas de error.
    /// Es un estándar común en las plantillas de proyecto de ASP.NET Core.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Obtiene o establece el identificador único de la solicitud HTTP que generó el error.
        /// Este ID ayuda a los administradores a rastrear el error en los logs del servidor.
        /// </summary>
        [Display(Name = "ID de Solicitud")]
        public string? RequestId { get; set; }

        /// <summary>
        /// Obtiene un valor que indica si el <see cref="RequestId"/> debe ser mostrado al usuario.
        /// Retorna <c>true</c> si el <see cref="RequestId"/> no es nulo ni está vacío.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
