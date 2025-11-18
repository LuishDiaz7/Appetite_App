namespace Appetite_App.Data
{
    public class DatabaseConnectionManager
    {
        // En un Singleton real, esta clase gestionaría la información de conexión.
        // Aquí, simplemente la usamos para demostrar el patrón de ciclo de vida.
        private readonly Guid _instanceId;

        public DatabaseConnectionManager()
        {
            _instanceId = Guid.NewGuid();
            // Lógica de conexión real iría aquí...
            Console.WriteLine($"DatabaseConnectionManager inicializado. ID: {_instanceId}");
        }

        public string GetConnectionStatus()
        {
            return $"Conexión activa. Instancia Singleton ID: {_instanceId}";
        }
    }
}
