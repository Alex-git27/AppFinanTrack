
using ManejoPresupuesto.Servicios;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);


//DEFINIMOS POLITIVA DE AUTORIZACION PARA USUARIO 
var politiaUsuariosAutenticados = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

// Add services to the container.
// Se utiliza "AddTransiente" ya que no comparte codigo entre distintas instancias del mismo servicio
builder.Services.AddControllersWithViews(opciones =>
{
    opciones.Filters.Add(new AuthorizeFilter(politiaUsuariosAutenticados));
});
builder.Services.AddTransient<IRepositorioTiposCuentas, RepositorioTiposCuentas>();// El "AddTransiente" es porque no comparte codigo entre distintas instancias del mismo servicio
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>(); // El "AddTransiente" es porque no comparte codigo entre distintas instancias del mismo servicio
builder.Services.AddTransient<IRepositorioCuentas, RepositorioCuentas>();
builder.Services.AddTransient<IRepositorioCategorias, RepositorioCategorias>(); //Importamos el servidio de IRepositorio de categorias.
builder.Services.AddTransient<IRepositorioTransacciones, RepositorioTransacciones>(); 
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IServicioReportes, ServicioReportes>();
builder.Services.AddTransient<IRepositorioUsuarios, RepositorioUsuarios>();
builder.Services.AddTransient<IUserStore<Usuario>, UsuarioStore>();
builder.Services.AddTransient<SignInManager<Usuario>>();
builder.Services.AddIdentityCore<Usuario>(opciones =>
{
    //opciones.Password.RequireDigit = false;
    //opciones.Password.RequireLowercase = false;
    //opciones.Password.RequireUppercase = false;
    //opciones.Password.RequireNonAlphanumeric = false;
}).AddErrorDescriber<MensajesDeErrorIdentity>();


//SERVICIO DE AUTENTICACION PARA QUE CUANDO LA APP RECIBA LA COOKIE PUEDA LEERLA
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;

    //DEFININIMOS EL SITIO DONDE EL USUARIO VA A REDIRECCIONARSE SI NO SE ENCUENTRA REGISTRADO
}).AddCookie(IdentityConstants.ApplicationScheme, opciones =>
{
    opciones.LoginPath = "/usuarios/login";
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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transacciones}/{action=Index}/{id?}");

app.Run();
