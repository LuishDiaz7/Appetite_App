using Microsoft.Extensions.Logging; 
using System; 
using System.Threading.Tasks;
using Appetite_App.Data;
using Appetite_App.Repositories;
using Appetite_App.Services;
using Appetite_App.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Appetite_App.Data.Repositories;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar DbContext con SQL Server
builder.Services.AddDbContext<AppetiteContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar SINGLETON 
builder.Services.AddSingleton<DatabaseConnectionManager>();

// Registrar repositorios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IOrdenRepository, OrdenRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProductoRepository, Appetite_App.Data.Repositories.ProductoRepository>();

// Registrar servicios
builder.Services.AddScoped<UserManagement>();
builder.Services.AddScoped<OrdenService>();
builder.Services.AddScoped<IProductoService, ProductoService>();

builder.Services.AddIdentity<Usuario, IdentityRole<int>>(options =>
{
    // Requisitos de Sign-in
    options.SignIn.RequireConfirmedAccount = false;

    // Requisitos de Contraseña TEMPORALMENTE RELAJADOS para asegurar la creación del usuario Admin
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6; // Longitud mínima de 6 caracteres
})
    .AddEntityFrameworkStores<AppetiteContext>()
    .AddDefaultTokenProviders();

// Configurar sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Inicializar base de datos con datos de prueba
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Se llama al inicializador para forzar el borrado y la recreación
        await DbInitializer.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); 

app.Run();
