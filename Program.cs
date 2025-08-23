var builder = WebApplication.CreateBuilder(args); // Crea el builder para la aplicación web

// Registrar el servicio de controladores con vistas (MVC)
builder.Services.AddControllersWithViews();

// Registrar autenticación con cookies
builder.Services.AddAuthentication("authCookie")
    .AddCookie("authCookie", options =>
    {
        options.LoginPath = "/"; // Ruta de login por defecto
    });

// Registrar autorización con roles y políticas
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole("administrador"));
    options.AddPolicy("Empleado", policy => policy.RequireRole("empleado"));
});

var app = builder.Build(); // Construye la aplicación

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Middleware para manejar errores en producción
    // El valor por defecto de HSTS es 30 días. Se recomienda cambiarlo en producción.
    app.UseHsts(); // Middleware para seguridad HTTP Strict Transport Security
}

app.UseHttpsRedirection(); // Redirige HTTP a HTTPS
app.UseStaticFiles(); // Sirve archivos estáticos (css, js, imágenes)
app.UseRouting(); // Habilita el enrutamiento
app.UseAuthentication(); // Habilita autenticación por cookies
app.UseAuthorization(); // Habilita autorización por roles y políticas

app.MapStaticAssets(); // Mapea los archivos estáticos

// Rutas personalizadas para propietarios
app.MapControllerRoute(
    name: "panel_propietarios_detalle",
    pattern: "panel/propietarios/{id}",
    defaults: new { controller = "Panel", action = "Propietario" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "panel_propietarios_lista",
    pattern: "panel/propietarios",
    defaults: new { controller = "Panel", action = "Propietarios" })
    .WithStaticAssets();

// Rutas personalizadas para inquilinos
app.MapControllerRoute(
    name: "panel_inquilinos_detalle",
    pattern: "panel/inquilinos/{id}",
    defaults: new { controller = "Panel", action = "Inquilino" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "panel_inquilinos_lista",
    pattern: "panel/inquilinos",
    defaults: new { controller = "Panel", action = "Inquilinos" })
    .WithStaticAssets();

// Rutas personalizadas para inmuebles
app.MapControllerRoute(
    name: "panel_inmuebles_detalle",
    pattern: "panel/inmuebles/{id}",
    defaults: new { controller = "Panel", action = "Inmueble" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "panel_inmuebles_lista",
    pattern: "panel/inmuebles",
    defaults: new { controller = "Panel", action = "Inquilinos" })
    .WithStaticAssets();

// Rutas personalizadas para contratos
app.MapControllerRoute(
    name: "panel_contratos_detalle",
    pattern: "panel/contratos/detalle/{id}",
    defaults: new { controller = "Contrato", action = "Detalle" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "panel_contratos_lista",
    pattern: "panel/contratos",
    defaults: new { controller = "Panel", action = "Contratos" })
    .WithStaticAssets();

// Rutas personalizadas para tipos de inmueble
app.MapControllerRoute(
    name: "panel_tipos_inmueble_detalle",
    pattern: "panel/tipos-inmueble/{id}",
    defaults: new { controller = "Panel", action = "TipoInmueble" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "panel_tipos_inmueble_lista",
    pattern: "panel/tipos-inmueble",
    defaults: new { controller = "Panel", action = "TiposInmueble" })
    .WithStaticAssets();

// Ruta para el panel principal
app.MapControllerRoute(
    name: "panel_index",
    pattern: "panel",
    defaults: new { controller = "Panel", action = "Index" })
    .WithStaticAssets();

// Ruta por defecto para el resto de controladores y acciones
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Autenticacion}/{action=Login}/{id?}")
    .WithStaticAssets();

// Middleware para manejar error 404
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        context.Response.Redirect("/Error404");
    }
});

app.Run(); // Inicia la aplicación
