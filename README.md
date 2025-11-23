# Appetite - Sistema de Gestión de Pedidos y E-commerce de Comida

## Descripción del Sistema

Este es un sistema backend robusto, desarrollado con **ASP.NET Core**, diseñado para gestionar un servicio de pedidos de comida tipo e-commerce para el restaurante Appetite. La aplicación permite a los usuarios **Clientes** explorar el catálogo de productos, realizar pedidos y seguir su estado en tiempo real, mientras que los usuarios **Administradores** gestionan el inventario, las categorías y supervisan el flujo completo de órdenes.

El enfoque principal del proyecto fue la aplicación rigurosa de **principios de Arquitectura de Software** y **Patrones de Diseño**, garantizando la mantenibilidad, escalabilidad y bajo acoplamiento del código.

### Objetivos Clave

- Gestión de usuarios por roles (Administrador y Cliente) y autenticación segura con Identity
- Administración dinámica de un catálogo de productos y categorías
- Implementación de un flujo complejo de creación y seguimiento de pedidos

---

## Arquitectura Utilizada

El sistema Appetite utiliza la **Arquitectura por Capas**. Esta estructura separa la funcionalidad en capas lógicas para aislar responsabilidades:

| Capa | Responsabilidad | Tecnologías Clave |
|------|----------------|-------------------|
| **Presentación/API** | Maneja solicitudes HTTP (endpoints), validación de DTOs, rutas y comunicación externa | ASP.NET Core Web API |
| **Servicios/Negocio** | Contiene la lógica central del negocio y la aplicación de todos los Patrones de Diseño | Servicios C# y Principios SOLID |
| **Persistencia/Datos** | Responsable de la comunicación con la base de datos (CRUD), migraciones y el DbContext | Entity Framework Core, SQL Server |

---

## Patrones y Principios de Diseño Aplicados

Para optimizar la estructura del código y cumplir con los requisitos de flexibilidad y extensibilidad, se implementaron los siguientes patrones:

### 1. Inyección de Dependencias (Dependency Injection - DI)

- **Propósito:** Reducir el acoplamiento entre clases al permitir que los objetos reciban sus dependencias (servicios, repositorios) a través de los constructores
- **Justificación:** Central en ASP.NET Core, facilita las pruebas unitarias y el cumplimiento del principio de Inversión de Dependencia (DIP) al trabajar con interfaces

### 2. Factory Method (Creacional)

- **Ubicación:** Servicios de gestión de usuarios (UserManagement) para crear diferentes tipos de usuarios (Administrador, Cliente)
- **Propósito:** Definir una interfaz para crear objetos de tipo Usuario, delegando la responsabilidad de la instanciación a subclases concretas (fábricas)
- **Justificación:** Desacopla la lógica de creación de roles del código que los utiliza (Principio OCP: Abierto a extensión, cerrado a modificación)

### 3. Builder (Creacional)

- **Ubicación:** Construcción de la entidad Pedido (por ejemplo, a través de IPedidoBuilder y una clase Director)
- **Propósito:** Separar la construcción de un objeto complejo (un pedido que incluye múltiples productos, datos de envío y pago) de su representación
- **Justificación:** Permite construir el Pedido paso a paso de forma controlada y garantiza que el objeto final sea válido

### 4. Decorator (Estructural)

- **Ubicación:** Lógica de cálculo de precios y funcionalidad de productos (e.g., clases como `QuesoExtraDecorator.cs`)
- **Propósito:** Añadir responsabilidades o funcionalidades (ej. ingredientes extra, envío especial) a objetos de forma dinámica y transparente
- **Justificación:** Permite extender la funcionalidad de un producto base sin modificar su clase original, siguiendo el principio de Composición sobre Herencia

### 5. Observer (Comportamiento)

- **Ubicación:** Gestión del estado de los pedidos y el sistema de notificaciones (`OrderSubject` e `IOrderObserver`)
- **Propósito:** Crear una dependencia uno-a-muchos, donde un objeto (Subject) notifica automáticamente a todos sus dependientes (Observers) cuando su estado cambia
- **Justificación:** Es esencial para el seguimiento de órdenes, ya que un cambio de estado ("Pendiente" a "Enviado") debe notificar al cliente, al módulo de cocina y al registro de logs simultáneamente

---

## Instrucciones para Ejecutar el Proyecto

Siga estos pasos para configurar y ejecutar la aplicación localmente:

### Prerrequisitos

- **SDK de .NET:** Versión 8.0 o superior
- **Base de Datos:** Acceso a una instancia de SQL Server

### Pasos

1. **Clonar el Repositorio:**
   ```bash
   git clone https://github.com/LuishDiaz7/Appetite_App.git
   cd Appetite_Project/Appetite_App
   ```

2. **Configurar la Base de Datos:**
   - Verifique la cadena de conexión en el archivo `appsettings.json` para que apunte a su instancia de SQL Server
   - **Aplicar Migraciones y Seeding:** El inicializador (`DbInitializer.cs`) aplicará las migraciones y sembrará los roles y el usuario administrador inicial automáticamente
   - Si prefiere hacerlo manualmente:
     ```bash
     dotnet ef database update
     ```

3. **Ejecutar la Aplicación:**
   ```bash
   dotnet run
   ```
   La aplicación se iniciará, típicamente en `https://localhost:5095`

### Credenciales de Prueba

Tras la inicialización, se crean los siguientes usuarios para pruebas:

| Rol | Email | Contraseña |
|-----|-------|------------|
| Administrador | admin2@appetite.com | AdminPassword123! |
| Cliente | cliente@appetite.com | ClientPassword123! |

---

## Dependencias Clave

El proyecto utiliza los siguientes paquetes NuGet esenciales:

- `Microsoft.NET.Sdk.Web`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- `Microsoft.AspNetCore.Mvc.NewtonsoftJson`

---

## Estructura del Proyecto

```
Appetite_App/
│
├── Controllers/
│   ├── AdminController.cs
│   ├── AuthController.cs
│   ├── CarritoController.cs
│   ├── CategoriaController.cs
│   ├── ClienteController.cs
│   ├── HomeController.cs
│   ├── OrdenController.cs
│   ├── ProductoController.cs
│   └── UsuarioController.cs
│
├── Models/
│   ├── Categoria.cs
│   ├── DetalleOrden.cs
│   ├── ItemCarrito.cs
│   ├── PreOrden.cs
│   ├── Producto.cs
│   └── Usuario.cs
│
├── Views/
│   ├── Admin/
│   │   ├── CrearUsuario.cshtml
│   │   ├── Index.cshtml
│   │   ├── Ordenes.cshtml
│   │   ├── Productos.cshtml
│   │   └── Usuarios.cshtml
│   │
│   ├── Auth/
│   │   ├── Login.cshtml
│   │   └── Registro.cshtml
│   │
│   ├── Carrito/
│   │   └── Index.cshtml
│   │
│   ├── Categoria/
│   │   ├── Crear.cshtml
│   │   ├── Editar.cshtml
│   │   └── Index.cshtml
│   │
│   ├── Cliente/
│   │   ├── Carrito.cshtml
│   │   ├── DetalleOrden.cshtml
│   │   ├── Index.cshtml
│   │   └── MisOrdenes.cshtml
│   │
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   │
│   ├── Producto/
│   │   ├── Catalogo.cshtml
│   │   ├── Crear.cshtml
│   │   ├── Detalle.cshtml
│   │   ├── Editar.cshtml
│   │   └── Index.cshtml
│   │
│   └── Shared/
│       ├── _Layout.cshtml
│       └── Error.cshtml
│
├── Services/
│   ├── IProductoService.cs
│   ├── OrdenService.cs
│   ├── ProductoService.cs
│   └── UserManagement.cs
│
├── Repositories/
│   ├── ICategoriaRepository.cs
│   ├── CategoriaRepository.cs
│   ├── IOrdenRepository.cs
│   ├── OrdenRepository.cs
│   ├── IProductoRepository.cs
│   ├── ProductoRepository.cs
│   ├── IUsuarioRepository.cs
│   └── UsuarioRepository.cs
│
├── Patterns/
│   ├── Factory/
│   │   ├── AdminFactory.cs
│   │   ├── ClientFactory.cs
│   │   └── UsuarioFactory.cs
│   │
│   ├── Builder/
│   │   ├── Director.cs
│   │   ├── IPedidoBuilder.cs
│   │   └── PreOrdenBuilder.cs
│   │
│   ├── Decorator/
│   │   ├── BebidaGrandeDecorator.cs
│   │   ├── CarneDobleDecorator.cs
│   │   ├── IProductoComponente.cs
│   │   ├── ProductoConcreto.cs
│   │   ├── ProductoDecorator.cs
│   │   └── QuesoExtraDecorator.cs
│   │
│   └── Observer/
│       ├── AuditorObserver.cs
│       ├── InventarioObserver.cs
│       ├── IOrderObserver.cs
│       ├── IOrderSubject.cs
│       ├── NotificacionObserver.cs
│       └── OrderSubject.cs
│
├── Data/
│   ├── AppetiteContext.cs
│   ├── DbInitializer.cs
│   └── Repositories/
│       └── ProductoRepository.cs
│
├── DTOs/
│   ├── CarritoItemDTO.cs
│   └── RegistroUsuarioDTO.cs
│
├── ViewModels/
│   ├── UsuarioListViewModel.cs
│   └── UsuarioViewModel.cs
│
├── wwwroot/
│   └── (archivos estáticos)
│
├── Program.cs
├── appsettings.json
└── Appetite_App.csproj
```

---

## Evidencias

### Login

<img width="1722" height="788" alt="image" src="https://github.com/user-attachments/assets/fa2f50a0-031e-4311-a475-ea048312e800" />

### Registro

<img width="1695" height="956" alt="image" src="https://github.com/user-attachments/assets/dfb4b3d3-f4ea-4ff1-8965-99d220d2dd47" />

### Panel del Administrador

<img width="1702" height="961" alt="image" src="https://github.com/user-attachments/assets/11a9fd83-b9d5-421f-b460-aa771e21aa37" />

### Gestión de Productos

<img width="1733" height="947" alt="image" src="https://github.com/user-attachments/assets/a6f24578-d314-4a0f-9820-21fedd7822b5" />

### Crear Nuevo Producto

<img width="1705" height="1000" alt="image" src="https://github.com/user-attachments/assets/ecf7edca-459d-4651-a3fe-895f3a2e5bde" />

### Gestión de Categorías

<img width="1692" height="843" alt="image" src="https://github.com/user-attachments/assets/90f83e31-6953-4da9-85fa-bdb8c473b3ed" />

### Crear Nueva Categoría

<img width="1673" height="967" alt="image" src="https://github.com/user-attachments/assets/21ab83c5-bf98-4b40-bde4-307281d64fb8" />

### Menú de Categorías (flujo del cliente)

<img width="1717" height="847" alt="image" src="https://github.com/user-attachments/assets/da5b0649-86ef-49c5-bae5-950796279e09" />

### Productos por Categoría 

<img width="1712" height="975" alt="image" src="https://github.com/user-attachments/assets/3b5c10e7-6d7f-4699-9774-2a0883536c30" />

### Detalle Producto

<img width="1718" height="1015" alt="image" src="https://github.com/user-attachments/assets/0f777ff7-8f7e-4a3a-b471-c81f905e11a2" />

### Carrito de Compras

<img width="1715" height="925" alt="image" src="https://github.com/user-attachments/assets/0959a406-1ff7-477b-bc29-99ba7ce80f54" />


### Documentación y Prototipo: 
https://docs.google.com/document/d/1q9pGZYPwhJ5uy5eXktQB7OVv3_td7MBb/edit?usp=sharing&ouid=104367277644571477006&rtpof=true&sd=true
### Sustentación:
https://drive.google.com/file/d/1nbL9umLl_BogCNJBDZM8D020Pt1eIPSQ/view?usp=sharing
