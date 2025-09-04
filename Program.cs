using inmobiliaria.Config;

var builder = WebApplication.CreateBuilder(args);

// Registrar el servicio de controladores con vistas (MVC)
builder.Services.AddControllersWithViews();

// Cookie
builder.Services.AddAuthentication("authCookie")
    .AddCookie("authCookie", options =>
    {
        options.LoginPath = "/"; // Ruta de login por defecto
    });

// Políticas
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole("administrador"));
    options.AddPolicy("Empleado", policy => policy.RequireRole("empleado"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // HTTP a HTTPS
app.UseStaticFiles(); // Archivos estáticos
app.UseRouting(); // Habilita el enrutamiento
app.UseAuthentication(); // Habilita autenticación por cookies
app.UseAuthorization(); // Habilita autorización por roles y políticas

app.MapStaticAssets(); // Mapea los archivos estáticos

// Router
RouteConfig.UseCustomRoutes(app);

// 404
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 401 && !context.Response.HasStarted)
    {
        context.Response.Redirect("/Error404");
    }
});

app.Run();
