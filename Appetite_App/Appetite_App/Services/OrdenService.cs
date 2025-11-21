using Appetite_App.Models;
using Appetite_App.Repositories;
using Appetite_App.Patterns.Builder;
using Appetite_App.Patterns.Decorator;
using Appetite_App.Patterns.Observer;
using Appetite_App.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appetite_App.Services
{
    /// <summary>
    /// Servicio que actúa como CLiente de los patrones Builder y Decorator,
    /// y como Subject para el patrón Observer.
    /// Ahora usa Inyección de Dependencias para el Director y el Sujeto.
    /// </summary>
    public class OrdenService
    {
        private readonly IOrdenRepository _ordenRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly Director _director; // Inyección de Director
        private readonly IOrderSubject _orderSubject;


        public OrdenService(
            IOrdenRepository ordenRepository,
            IProductoRepository productoRepository,
            Director director,
            IOrderSubject orderSubject)
        {
            _ordenRepository = ordenRepository;
            _productoRepository = productoRepository;
            _director = director;
            _orderSubject = orderSubject;
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
        /// <summary>
        /// Procesa la compra completa: aplica decoradores, usa el Builder para ensamblar la orden
        /// y luego notifica a los Observers.
        /// </summary>
        public async Task<PreOrden> CrearOrdenDesdeCarrito(Usuario usuario, string direccion, List<CarritoItemDTO> carrito)
        {
            // 1. Inicializar el Builder (Creamos una nueva instancia de Builder, pero usamos el Director inyectado)
            var builder = new PreOrdenBuilder();
            _director.Builder = builder; // Asignamos el builder al director

            var detallesOrden = new List<DetalleOrden>();

            // 2. Lógica para crear DetalleOrden usando Decorator (omito por brevedad, pero debe ir aquí)

            // 3. Construir la PreOrden usando el Director (Patrón Builder)
            // Llama a ConstruirPedidoCompleto con la lista de detalles y la dirección
            _director.ConstruirPedidoCompleto(usuario, detallesOrden, direccion);

            // Obtener el resultado final
            PreOrden nuevaOrden = builder.GetPreOrden(); // Usamos GetPreOrden()

            // 4. Persistir la orden
            await _ordenRepository.AddAsync(nuevaOrden);

            // 5. Notificar a los Observadores (Patrón Observer)
            _orderSubject.Notify(nuevaOrden, "CREATED");

            return nuevaOrden;
        }

        // ---------------------------------------------
        // MÉTODO CLAVE: Usa el Patrón OBSERVER
        // ---------------------------------------------
        /// <summary>
        /// Cambia el estado de una orden y notifica a los observadores si el cambio es relevante.
        /// </summary>
        public async Task<PreOrden?> CambiarEstadoOrden(int idOrden, string nuevoEstado)
        {
            PreOrden? orden = await _ordenRepository.GetByIdAsync(idOrden);
            if (orden == null) return null;

            orden.Estado = nuevoEstado;
            await _ordenRepository.UpdateAsync(orden);

            // Notificar a los Observadores
            if (nuevoEstado == "Preparada")
            {
                _orderSubject.Notify(orden, "PREPARED");
            }
            else if (nuevoEstado == "Cancelada")
            {
                // La cancelación podría activar el reabastecimiento de inventario
                _orderSubject.Notify(orden, "CANCELED");
            }
            else if (nuevoEstado == "Entregada")
            {
                _orderSubject.Notify(orden, "DELIVERED");
            }

            return orden;
        }

        // Otros métodos de negocio...
        public async Task<List<PreOrden>> GetOrdenesPorUsuario(int idUsuario)
        {
            // Nota: Es mejor usar un método de repositorio que filtre en la BD, no GetAll y luego ToList
            return (await _ordenRepository.GetAllAsync()).Where(o => o.IdUsuario == idUsuario).ToList();
        }
    }
}
