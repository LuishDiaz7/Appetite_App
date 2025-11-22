using Appetite_App.Data;
using Appetite_App.Data.Repositories;
using Appetite_App.Models;
using Appetite_App.Patterns.Builder;
using Appetite_App.Patterns.Observer; // Necesario para el Patrón Observer
using Appetite_App.Repositories;
using Appetite_App.Services;
using Microsoft.AspNetCore.Authentication.Cookies; // No utilizado explícitamente, pero es parte de Identity
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// Crea el constructor de la aplicación web
var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN BÁSICA ---

// Agrega servicios MVC para manejar controladores y vistas.
builder.Services.AddControllersWithViews();

// Configura DbContext con SQL Server (Scoped por defecto).
// Esto conecta la aplicación a la base de datos.
builder.Services.AddDbContext<AppetiteContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. REGISTRO DE REPOSITORIOS Y SERVICIOS CORE ---

// Registrar repositorios (Patrón Repository - Abstracción de acceso a datos)
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IOrdenRepository, OrdenRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProductoRepository, Appetite_App.Data.Repositories.ProductoRepository>();

// Registrar servicios de negocio
builder.Services.AddScoped<OrdenService>();
builder.Services.AddScoped<IProductoService, ProductoService>();

// --- 3. REGISTRO DE PATRONES DE DISEÑO ---

// Patrón Builder: El Director se registra para ser inyectado en OrdenService.
builder.Services.AddScoped<Director>();

// Patrón Observer:
// 1. Registrar el Sujeto Concreto (OrderSubject) como la implementación de IOrderSubject.
builder.Services.AddScoped<IOrderSubject, OrderSubject>();

// 2. Registrar todos los Observadores Concretos contra su interfaz (IOrderObserver).
// El contenedor de DI inyectará automáticamente un IEnumerable<IOrderObserver> en el constructor
// de OrderSubject, incluyendo todas las implementaciones registradas aquí.
builder.Services.AddScoped<IOrderObserver, InventarioObserver>();
builder.Services.AddScoped<IOrderObserver, AuditorObserver>(); // Añadido para completar el patrón Observer

// --- 4. CONFIGURACIÓN DE ASP.NET CORE IDENTITY Y SESIÓN ---

builder.Services.AddIdentity<Usuario, IdentityRole<int>>(options =>
{
    // Requisitos de Sign-in
    options.SignIn.RequireConfirmedAccount = false;

    // Requisitos de Contraseña (Se deben ajustar los requisitos en producción)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<AppetiteContext>()
    .AddDefaultTokenProviders();

// Configuración de almacenamiento en caché para datos de sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true; // La cookie solo es accesible por el lado del servidor
    options.Cookie.IsEssential = true; // Necesaria para el funcionamiento de la aplicación
});

// Construir la aplicación
var app = builder.Build();

// --- 5. INICIALIZACIÓN DE LA BASE DE DATOS ---

// Ejecutar operaciones asíncronas de migración e inicialización de datos (seed)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Inicializar base de datos con datos de prueba
        await DbInitializer.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
    }
}

// --- 6. PIPELINE DE MIDDLEWARE (Orden crucial) ---

// Configurar el pipeline de solicitud HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// CRÍTICO para servir imágenes y CSS/JS desde wwwroot
app.UseStaticFiles();

app.UseRouting();

// El orden de estos middlewares es CRÍTICO:
app.UseSession();
app.UseAuthentication(); // Debe venir antes de UseAuthorization
app.UseAuthorization();

// Mapeo de rutas MVC (la ruta por defecto)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Iniciar la aplicación
app.Run();