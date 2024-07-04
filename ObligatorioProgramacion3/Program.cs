using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using ObligatorioProgramacion3.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ObligatorioProgramacion3Context>(options =>
options.UseSqlServer());
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
    options.LoginPath = "/Usuarios/Login";
    options.LogoutPath = "/Home/Index";
    options.AccessDeniedPath = "/Home/AccesoDenegado";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(100);
    options.SlidingExpiration = true;
    });

builder.Services.AddScoped<IAuthorizationHandler, PermisoPaginaManejador>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<CarritoService>();
builder.Services.AddAuthorization(options =>
{

    options.AddPolicy("PoderPagarReserva", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PoderPagarReserva")));
    // Define pol�ticas para AdministracionController
    options.AddPolicy("AdministracionVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("AdministracionVer")));
    
    // Define pol�ticas para ClientesController
    options.AddPolicy("ClientesVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClientesVer")));
    options.AddPolicy("ClientesCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClientesCrear")));
    options.AddPolicy("ClientesEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClientesEditar")));
    options.AddPolicy("ClientesEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClientesEliminar")));
    options.AddPolicy("ClientesDetalle", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClientesDetalle")));

    // Define pol�ticas para ClimasController
    options.AddPolicy("ClimasVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClimasVer")));
    options.AddPolicy("ClimasCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClimasCrear")));
    options.AddPolicy("ClimasEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClimasEditar")));
    options.AddPolicy("ClimasEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClimasEliminar")));
    options.AddPolicy("ClimasDetalle", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClimasDetalle")));

    // Define pol�ticas para MesasController
    options.AddPolicy("MesasVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("MesasVer")));
    options.AddPolicy("MesasCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("MesasCrear")));
    options.AddPolicy("MesasEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("MesasEditar")));
    options.AddPolicy("MesasEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("MesasEliminar")));
    options.AddPolicy("MesasDetalle", policy => policy.Requirements.Add(new PermisoPaginaRequisito("MesasDetalle")));

    // Define pol�ticas para OrdenDetallesController
    options.AddPolicy("OrdenDetallesVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenDetallesVer")));
    options.AddPolicy("OrdenDetallesCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenDetallesCrear")));
    options.AddPolicy("OrdenDetallesEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenDetallesEditar")));
    options.AddPolicy("OrdenDetallesEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenDetallesEliminar")));
    options.AddPolicy("OrdenDetallesDetalle", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenDetallesDetalle")));

    // Define pol�ticas para OrdenesController
    options.AddPolicy("OrdenesVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenesVer")));
    options.AddPolicy("OrdenesCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenesCrear")));
    options.AddPolicy("OrdenesEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenesEditar")));
    options.AddPolicy("OrdenesEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenesEliminar")));
    options.AddPolicy("OrdenesDetalle", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenesDetalle")));

    // Define pol�ticas para PagosController
    options.AddPolicy("PagosVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PagosVer")));
    options.AddPolicy("PagosCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PagosCrear")));
    options.AddPolicy("PagosEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PagosEditar")));
    options.AddPolicy("PagosEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PagosEliminar")));
    options.AddPolicy("PagosDetalle", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PagosDetalle")));
    options.AddPolicy("PagosPagarReserva", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PagosPagarReserva")));

    // Define pol�ticas para PlatoesController
    options.AddPolicy("PlatoesVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PlatoesVer")));
    options.AddPolicy("PlatoesSoloVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PlatoesSoloVer")));
    options.AddPolicy("PlatoesCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PlatoesCrear")));
    options.AddPolicy("PlatoesEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PlatoesEditar")));
    options.AddPolicy("PlatoesEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PlatoesEliminar")));
    options.AddPolicy("PlatoesSeleccionarPlato", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PlatoesSeleccionarPlato")));

    // Define pol�ticas para Rese�asController
    options.AddPolicy("Rese�asVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("Rese�asVer")));
    options.AddPolicy("Rese�asCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("Rese�asCrear")));
    options.AddPolicy("Rese�asEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("Rese�asEditar")));
    options.AddPolicy("Rese�asEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("Rese�asEliminar")));
    options.AddPolicy("Rese�asCrearRese�aUsuario", policy => policy.Requirements.Add(new PermisoPaginaRequisito("Rese�asCrearRese�aUsuario")));
    options.AddPolicy("Rese�asRese�as", policy => policy.Requirements.Add(new PermisoPaginaRequisito("Rese�asRese�as")));
    options.AddPolicy("Rese�asDetalle", policy => policy.Requirements.Add(new PermisoPaginaRequisito("Rese�asDetalle")));

    // Define pol�ticas para ReservasController
    options.AddPolicy("ReservasVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReservasVer")));
    options.AddPolicy("ReservasCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReservasCrear")));
    options.AddPolicy("ReservasEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReservasEditar")));
    options.AddPolicy("ReservasEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReservasEliminar")));
    options.AddPolicy("ReservasDetalle", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReservasDetalle")));
    options.AddPolicy("ReservasCrearReserva", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReservasCrearReserva")));
    options.AddPolicy("ReservasMostrarReservas", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReservasMostrarReservas")));
    options.AddPolicy("ReservasSeleccionarFecha", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReservasSeleccionarFecha")));
    options.AddPolicy("ReservasSeleccionarMesa", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReservasSeleccionarMesa")));


    // Define pol�ticas para RestaurantesController
    options.AddPolicy("RestaurantesVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RestaurantesVer")));
    options.AddPolicy("RestaurantesCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RestaurantesCrear")));
    options.AddPolicy("RestaurantesEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RestaurantesEditar")));
    options.AddPolicy("RestaurantesEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RestaurantesEliminar")));
    options.AddPolicy("RestaurantesDetalle", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RestaurantesDetalle")));
    options.AddPolicy("RestaurantesSeleccionarRestaurante", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RestaurantesSeleccionarRestaurante")));


    // Define pol�ticas para UsuariosController
    options.AddPolicy("UsuariosVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("UsuariosVer")));
    options.AddPolicy("UsuariosCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("UsuariosCrear")));
    options.AddPolicy("UsuariosEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("UsuariosEditar")));
    options.AddPolicy("UsuariosEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("UsuariosEliminar")));
    options.AddPolicy("UsuariosDetalle", policy => policy.Requirements.Add(new PermisoPaginaRequisito("UsuariosDetalle")));
    options.AddPolicy("UsuariosLogin", policy => policy.Requirements.Add(new PermisoPaginaRequisito("UsuariosLogin")));
    options.AddPolicy("UsuariosRegistroUsuario", policy => policy.Requirements.Add(new PermisoPaginaRequisito("UsuariosRegistroUsuario")));


    // Pol�ticas para roles
    options.AddPolicy("RolesVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RolesVer")));
    options.AddPolicy("RolesCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RolesCrear")));
    options.AddPolicy("RolesEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RolesEditar")));
    options.AddPolicy("RolesEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RolesEliminar")));
    options.AddPolicy("RolesDetalle", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RolesDetalle")));

    // Pol�ticas para permisos
    options.AddPolicy("PermisosVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PermisosVer")));
    options.AddPolicy("PermisosCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PermisosCrear")));
    options.AddPolicy("PermisosEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PermisosEditar")));
    options.AddPolicy("PermisosEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PermisosEliminar")));
    options.AddPolicy("PermisosDetalle", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PermisosDetalle")));
    options.AddPolicy("PermisosRolesYPermisos", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PermisosRolesYPermisos")));

});
builder.Services.AddControllersWithViews();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

