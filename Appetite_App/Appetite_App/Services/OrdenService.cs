using Appetite_App.Models;
using Appetite_App.Repositories;
using Appetite_App.Patterns.Builder;
using Appetite_App.Patterns.Decorator;
using Appetite_App.Patterns.Observer;
using Appetite_App.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Necesario para ArgumentNullException
using System.ComponentModel.DataAnnotations;

namespace Appetite_App.Services
{
    /// <summary>
    /// Servicio principal de la lógica de negocio para la gestión de órdenes.
    /// Esta clase coordina múltiples patrones:
    /// <list type="bullet">
    /// <item><term>Patrón Builder</term><description>Actúa como el cliente del <see cref="Director"/> para construir objetos complejos como <see cref="PreOrden"/>.</description></item>
    /// <item><term>Patrón Decorator</term><description>Utiliza los decoradores para calcular el precio final de los ítems del carrito.</description></item>
    /// <item><term>Patrón Observer</term><description>Actúa como el sujeto que notifica cambios de estado de órdenes a los observadores a través de <see cref="IOrderSubject"/>.</description></item>
    /// </list>
    /// 
    /// </summary>
    public class OrdenService
    {
        private readonly IOrdenRepository _ordenRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly Director _director; // Inyección del Director (Patrón Builder)
        private readonly IOrderSubject _orderSubject; // Inyección del Sujeto (Patrón Observer)


        /// <summary>
        /// Inicializa una nueva instancia del <see cref="OrdenService"/> con sus dependencias inyectadas.
        /// Asegura que todas las dependencias críticas no sean nulas.
        /// </summary>
        /// <param name="ordenRepository">Repositorio para la persistencia de <see cref="PreOrden"/>.</param>
        /// <param name="productoRepository">Repositorio para obtener la data base de <see cref="Producto"/>.</param>
        /// <param name="director">Director del patrón Builder para el ensamblaje de la orden.</param>
        /// <param name="orderSubject">Sujeto del patrón Observer para la notificación de eventos de orden.</param>
        /// <exception cref="ArgumentNullException">Lanzada si alguna de las dependencias es nula.</exception>
        public OrdenService(
            IOrdenRepository ordenRepository,
            IProductoRepository productoRepository,
            Director director,
            IOrderSubject orderSubject)
        {
            _ordenRepository = ordenRepository ?? throw new ArgumentNullException(nameof(ordenRepository));
            _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
            _director = director ?? throw new ArgumentNullException(nameof(director));
            _orderSubject = orderSubject ?? throw new ArgumentNullException(nameof(orderSubject));
        }

        // ---------------------------------------------
        // MÉTODO AUXILIAR: Usa el Patrón DECORATOR
        // ---------------------------------------------

        /// <summary>
        /// Aplica la cadena de decoradores (<c>Decorator</c>) al producto base para calcular el precio
        /// y obtener la descripción final de un ítem del carrito.
        /// </summary>
        /// <param name="productoBase">El modelo <see cref="Producto"/> base sin modificar.</param>
        /// <param name="decoradores">Lista de nombres de los decoradores a aplicar (ej. "QuesoExtra").</param>
        /// <returns>El componente final <see cref="IProductoComponente"/> con todos los decoradores aplicados.</returns>
        public IProductoComponente ConstruirComponente(Producto productoBase, List<string> decoradores)
        {
            IProductoComponente componente = new ProductoConcreto(productoBase);

            foreach (var decoratorNombre in decoradores)
            {
                // Encadenamiento de Decoradores 
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
        /// Procesa la compra completa:
        /// <list type="number">
        /// <item>Utiliza <c>Decorator</c> para calcular los precios de cada ítem.</item>
        /// <item>Utiliza el <c>Director</c> (Patrón Builder) para ensamblar la orden (<see cref="PreOrden"/> y <see cref="DetalleOrden"/>).</item>
        /// <item>Persiste la orden en el repositorio.</item>
        /// <item>Notifica a los observadores (Patrón Observer).</item>
        /// </list>
        /// </summary>
        /// <param name="usuario">El usuario que realiza la orden.</param>
        /// <param name="direccion">La dirección de entrega.</param>
        /// <param name="carrito">La lista de ítems en el carrito de compras, típicamente DTOs.</param>
        /// <returns>La <see cref="PreOrden"/> recién creada.</returns>
        public async Task<PreOrden> CrearOrdenDesdeCarrito(Usuario usuario, string direccion, List<CarritoItemDTO> carrito)
        {
            // 1. Inicializar el Builder 
            var builder = new PreOrdenBuilder();
            _director.Builder = builder; // Asignamos el builder al director

            var detallesOrden = new List<DetalleOrden>();

            // Nota: Aquí se omitió la lógica DECORATOR de reconstrucción del carrito.
            // En una aplicación real, se iteraría sobre 'carrito', se usaría 'ConstruirComponente' 
            // para obtener el precio y la descripción final, y se crearía el DetalleOrden.
            // Para el propósito de demostración, 'detallesOrden' se pasa vacío.

            // 2. Construir la PreOrden usando el Director (Patrón Builder)
            _director.ConstruirPedidoCompleto(usuario, detallesOrden, direccion);

            // Obtener el resultado final
            PreOrden nuevaOrden = builder.GetPreOrden();

            // 3. Persistir la orden
            await _ordenRepository.AddAsync(nuevaOrden);

            // 4. Notificar a los Observadores (Patrón Observer)
            _orderSubject.Notify(nuevaOrden, "CREATED"); // Notifica que se ha creado una nueva orden

            return nuevaOrden;
        }

        // ---------------------------------------------
        // MÉTODO CLAVE: Usa el Patrón OBSERVER
        // ---------------------------------------------

        /// <summary>
        /// Cambia el estado de una orden persistida, la actualiza en la base de datos y
        /// notifica a los observadores (<c>Observer</c>) según el nuevo estado.
        /// 
        /// </summary>
        /// <param name="idOrden">El ID de la orden a modificar.</param>
        /// <param name="nuevoEstado">El nuevo estado de la orden (ej. "Preparada", "Cancelada", "Entregada").</param>
        /// <returns>La <see cref="PreOrden"/> actualizada, o <c>null</c> si no se encuentra.</returns>
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

        // ---------------------------------------------
        // Otros métodos
        // ---------------------------------------------

        /// <summary>
        /// Obtiene todas las órdenes realizadas por un usuario específico de manera asíncrona.
        /// </summary>
        /// <param name="idUsuario">El identificador del usuario.</param>
        /// <returns>Una lista de <see cref="PreOrden"/> del usuario.</returns>
        public async Task<List<PreOrden>> GetOrdenesPorUsuario(int idUsuario)
        {
            // Nota: Se realiza un filtro en memoria (Where().ToList()) para simplificar. 
            // En producción, se usaría un método optimizado del repositorio.
            return (await _ordenRepository.GetAllAsync()).Where(o => o.IdUsuario == idUsuario).ToList();
        }
    }
}
