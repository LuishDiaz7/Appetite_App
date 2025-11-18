namespace Appetite_App.DTOs
{
    public class CarritoItemDTO
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        // Lista de decoradores a aplicar (ej: "QuesoExtra", "CarneDoble")
        public List<string> Decoradores { get; set; } = new List<string>();
    }
}
