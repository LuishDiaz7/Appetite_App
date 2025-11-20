using Microsoft.AspNetCore.Identity;

namespace Appetite_App.Models
{

    public class Usuario : IdentityUser<int>
    {

        public string Nombre { get; set; } = string.Empty;
        public ICollection<PreOrden> Ordenes { get; set; } = new List<PreOrden>();
    }
}