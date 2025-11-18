using Appetite_App.Models;
using Appetite_App.Repositories;
using Appetite_App.Patterns.Builder;
using Appetite_App.Patterns.Decorator;
using Appetite_App.Patterns.Observer;
using Appetite_App.DTOs; // Necesario para CarritoItemDTO

namespace Appetite_App.Services
{
    /// <summary>
    /// Servicio que actúa como CLiente de los patrones Builder y Decorator,
    /// y como Subject para el patrón Observer.
    /// </summary>
    public class OrdenService
    {
        private readonly IOrdenRepository _ordenRepository;
        private readonly Director _director;
        private readonly OrderSubject _orderSubject;
        private readonly IProductoRepository _productoRepository;

        public OrdenService(
            IOrdenRepository ordenRepository,
            IProductoRepository productoRepository)
        {
            _ordenRepository = ordenRepository;
            _productoRepository = productoRepository;
            _director = new Director();
            _orderSubject = new OrderSubject();

            // Inicializar Observadores
            // La inyección de dependencias (DI) es la forma correcta, pero para el prototipo:
            _orderSubject.Attach(new InventarioObserver());
            _orderSubject.Attach(new AuditorObserver());
            _orderSubject.Attach(new NotificacionObserver());
        }

        // ---------------------------------------------
        // MÉTODO AUXILIAR: Usa el Patrón DECORATOR (para el carrito)
        // ---------------------------------------------
        /// <summary>
        /// Aplica los decoradores al producto base.
        /// Este método es usado por el ClienteController para calcular precios en tiempo real.
        /// </summary>
        public IProductoComponente ConstruirComponente(Producto productoBase, List<string> decoradores)
        {
            IProductoComponente componente = new ProductoConcreto(productoBase);

            foreach (var decoratorNombre in decoradores)
            {
                componente = decoratorNombre switch
                {
                    "QuesoExtra" => new QuesoExtraDecorator(componente),
                    "CarneDoble" => new CarneDobleDecorator(componente),
                    "BebidaGrande" => new BebidaGrandeDecorator(componente),
                    _ => componente,
                };
            }
            return componente;
        }

        // ---------------------------------------------
        // MÉTODO CLAVE: Usa los patrones DECORATOR y BUILDER
        // ---------------------------------------------
        public async Task<PreOrden> CrearOrdenDesdeCarrito(Usuario usuario, string direccion, List<CarritoItemDTO> carrito)
        {
            var builder = new PreOrdenBuilder();
            _director.Builder = builder;

            var detallesOrden = new List<DetalleOrden>();

            // 1. Aplicar Decorator y construir Detalles
            foreach (var item in carrito)
            {
                // Producto base
                Producto? productoBase = await _productoRepository.GetByIdAsync(item.IdProducto);
                if (productoBase == null) continue;

                // Aplicar el Patrón Decorator usando el método auxiliar
                IProductoComponente componente = ConstruirComponente(productoBase, item.Decoradores);

                // Crear DetalleOrden con la información decorada
                var detalle = new DetalleOrden
                {
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad,
                    // PrecioUnitario y Subtotal usan el precio final decorado
                    PrecioUnitario = componente.GetPrecio(),
                    Subtotal = componente.GetPrecio() * item.Cantidad,
                    DecoradoresAplicados = componente.GetDescripcion() // Descripción decorada
                };
                detallesOrden.Add(detalle);
            }

            // 2. Construir la Orden usando el Patrón Builder/Director
            PreOrden nuevaOrden = _director.ConstruirOrden(usuario, direccion, detallesOrden);

            // 3. Persistir la orden
            await _ordenRepository.AddAsync(nuevaOrden); // CORREGIDO: Usar AddAsync

            // 4. Notificar a los Observadores (Patrón Observer)
            _orderSubject.Notify(nuevaOrden, "Created");

            return nuevaOrden;
        }

        // ---------------------------------------------
        // MÉTODO CLAVE: Usa el Patrón OBSERVER
        // ---------------------------------------------
        public async Task<PreOrden?> CambiarEstadoOrden(int idOrden, string nuevoEstado)
        {
            PreOrden? orden = await _ordenRepository.GetByIdAsync(idOrden); // CORREGIDO: Usar GetByIdAsync
            if (orden == null) return null;

            orden.Estado = nuevoEstado;
            await _ordenRepository.UpdateAsync(orden); // CORREGIDO: Usar UpdateAsync

            // Notificar a los Observadores
            if (nuevoEstado == "Preparada")
            {
                _orderSubject.Notify(orden, "Prepared");
            }
            else if (nuevoEstado == "Cancelada")
            {
                _orderSubject.Notify(orden, "Canceled");
            }

            return orden;
        }

        // Otros métodos de negocio...
        public async Task<List<PreOrden>> GetOrdenesPorUsuario(int idUsuario)
        {
            return (await _ordenRepository.GetAllAsync()).Where(o => o.IdUsuario == idUsuario).ToList(); // CORREGIDO: Usar GetAllAsync
        }
    }
}
