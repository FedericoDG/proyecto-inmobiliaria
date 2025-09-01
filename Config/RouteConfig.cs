namespace inmobiliaria.Config
{
    public static class RouteConfig
    {
        public static void UseCustomRoutes(this IEndpointRouteBuilder app)
        {
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

            // Rutas personalizadas para pagos
            app.MapControllerRoute(
                name: "panel_pagos_detalle",
                pattern: "panel/pagos/detalle/{id}",
                defaults: new { controller = "Pago", action = "Detalle" })
                .WithStaticAssets();

            app.MapControllerRoute(
                name: "panel_pagos_lista",
                pattern: "panel/pagos",
                defaults: new { controller = "Pago", action = "Index" })
                .WithStaticAssets();

            // Rutas personalizadas para usuarios
            app.MapControllerRoute(
                name: "panel_usuarios_detalle",
                pattern: "panel/usuarios/{id}",
                defaults: new { controller = "Panel", action = "Usuario" })
                .WithStaticAssets();

            app.MapControllerRoute(
                name: "panel_usuarios_lista",
                pattern: "panel/usuarios",
                defaults: new { controller = "Panel", action = "Usuarios" })
                .WithStaticAssets();

            // Ruta para el panel principal
            app.MapControllerRoute(
                name: "panel_index",
                pattern: "panel",
                defaults: new { controller = "Panel", action = "Index" })
                .WithStaticAssets();

            app.MapControllerRoute(
                name: "panel_perfil",
                pattern: "panel/perfil",
                defaults: new { controller = "Panel", action = "Perfil" })
                .WithStaticAssets();


            // Ruta por defecto para el resto de controladores y acciones
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Autenticacion}/{action=Login}/{id?}")
                .WithStaticAssets();
        }
    }
}
