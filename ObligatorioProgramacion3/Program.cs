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
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<CarritoService>();
builder.Services.AddAuthorization(options =>
{
    // Define políticas para AdministracionController
    options.AddPolicy("AdministracionVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("AdministracionVer")));
    
    // Define políticas para ClientesController
    options.AddPolicy("ClientesVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClientesVer")));
    options.AddPolicy("ClientesCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClientesCrear")));
    options.AddPolicy("ClientesEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClientesEditar")));
    options.AddPolicy("ClientesEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClientesEliminar")));

    // Define políticas para ClimasController
    options.AddPolicy("ClimasVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClimasVer")));
    options.AddPolicy("ClimasCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClimasCrear")));
    options.AddPolicy("ClimasEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClimasEditar")));
    options.AddPolicy("ClimasEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ClimasEliminar")));

    // Define políticas para MesasController
    options.AddPolicy("MesasVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("MesasVer")));
    options.AddPolicy("MesasCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("MesasCrear")));
    options.AddPolicy("MesasEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("MesasEditar")));
    options.AddPolicy("MesasEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("MesasEliminar")));

    // Define políticas para OrdenDetallesController
    options.AddPolicy("OrdenDetallesVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenDetallesVer")));
    options.AddPolicy("OrdenDetallesCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenDetallesCrear")));
    options.AddPolicy("OrdenDetallesEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenDetallesEditar")));
    options.AddPolicy("OrdenDetallesEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenDetallesEliminar")));

    // Define políticas para OrdenesController
    options.AddPolicy("OrdenesVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenesVer")));
    options.AddPolicy("OrdenesCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenesCrear")));
    options.AddPolicy("OrdenesEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenesEditar")));
    options.AddPolicy("OrdenesEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("OrdenesEliminar")));

    // Define políticas para PagosController
    options.AddPolicy("PagosVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PagosVer")));
    options.AddPolicy("PagosCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PagosCrear")));
    options.AddPolicy("PagosEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PagosEditar")));
    options.AddPolicy("PagosEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PagosEliminar")));

    // Define políticas para PlatoesController
    options.AddPolicy("PlatoesVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PlatoesVer")));
    options.AddPolicy("PlatoesCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PlatoesCrear")));
    options.AddPolicy("PlatoesEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PlatoesEditar")));
    options.AddPolicy("PlatoesEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("PlatoesEliminar")));

    // Define políticas para ReseñasController
    options.AddPolicy("ReseñasVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReseñasVer")));
    options.AddPolicy("ReseñasCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReseñasCrear")));
    options.AddPolicy("ReseñasEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReseñasEditar")));
    options.AddPolicy("ReseñasEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReseñasEliminar")));

    // Define políticas para ReservasController
    options.AddPolicy("ReservasVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReservasVer")));
    options.AddPolicy("ReservasCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReservasCrear")));
    options.AddPolicy("ReservasEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReservasEditar")));
    options.AddPolicy("ReservasEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("ReservasEliminar")));

    // Define políticas para RestaurantesController
    options.AddPolicy("RestaurantesVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RestaurantesVer")));
    options.AddPolicy("RestaurantesCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RestaurantesCrear")));
    options.AddPolicy("RestaurantesEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RestaurantesEditar")));
    options.AddPolicy("RestaurantesEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("RestaurantesEliminar")));

    // Define políticas para UsuariosController
    options.AddPolicy("UsuariosVer", policy => policy.Requirements.Add(new PermisoPaginaRequisito("UsuariosVer")));
    options.AddPolicy("UsuariosCrear", policy => policy.Requirements.Add(new PermisoPaginaRequisito("UsuariosCrear")));
    options.AddPolicy("UsuariosEditar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("UsuariosEditar")));
    options.AddPolicy("UsuariosEliminar", policy => policy.Requirements.Add(new PermisoPaginaRequisito("UsuariosEliminar")));

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
app.Use(async (context, next) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        var userPermissions = context.User.Claims
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .ToList();

        context.Items["UserPermissions"] = userPermissions;
    }
    await next.Invoke();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

