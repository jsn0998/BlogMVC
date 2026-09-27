using BlogMVC.Datos;
using BlogMVC.Entidades;
using BlogMVC.Servicios;
using BlogMVC.Utilidades;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/* Configuracion de tecnologia razor para cargarlo en el servidor */
builder.Services.AddServerSideBlazor();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();

/*
    Se usa AddDbContextFactory porque en este proyecto se integrara o inyectara el DbContext en Blazor
    Es una factoria que permite dentro de un componente de Blazor construir un DbContext sirve como para poder inyectarlo en un controlador
*/
builder.Services.AddDbContextFactory<ApplicationDbContext>(opciones =>
{
    opciones.UseSqlServer("name=DefaultConnection")

    /* Tener la logica de DataSeeding en una clase especifica */
    .UseSeeding(Seeding.Aplicar)
    .UseAsyncSeeding(Seeding.AplicarAsync);
});

/* 
 Esto permite que cuando una persona se registre puede simplemente loguearse y usar la aplicacion 
 Para utilizar Identity con ese DbContext
*/
builder.Services.AddIdentity<Usuario, IdentityRole>(opciones =>
{
    opciones.SignIn.RequireConfirmedAccount = false;// Significa requerir una cuenta confirmada para evitar confirmar la cuenta con un correo real
}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

/*
 Esto permite configurar las url por defecto de login
*/
builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, opciones => {
    /* Rutas de redireccion para negar el acceso a usuarios sin permiso */
    opciones.LoginPath = "/usuarios/login";
    opciones.AccessDeniedPath = "/usuarios/login";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

/* Para manejar todo lo relacionado a las peticiones de razor (componentes de blazor) */
app.MapBlazorHub();


app.Run();
