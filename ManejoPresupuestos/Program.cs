using ManejoPresupuestos.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();// El "AddTransiente" es porque no comparte codigo entre distintas instancias del mismo servicio
builder.Services.AddTransient<IRepositorioTiposCuentas, RepositorioTiposCuentas>();// El "AddTransiente" es porque no comparte codigo entre distintas instancias del mismo servicio
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>(); // El "AddTransiente" es porque no comparte codigo entre distintas instancias del mismo servicio
builder.Services.AddTransient<IRepositorioCuentas, RepositorioCuentas>();
builder.Services.AddTransient<IRepositorioCategorias, RepositorioCategorias>(); //Importamos el servidio de IRepositorio de categorias.
builder.Services.AddTransient<IRepositorioTransacciones, RepositorioTransacciones>(); 
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IServicioReportes, ServicioReportes>();
builder.Services.AddTransient<IRepositorioUsuarios, RepositorioUsuarios>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transacciones}/{action=Index}/{id?}");

app.Run();
