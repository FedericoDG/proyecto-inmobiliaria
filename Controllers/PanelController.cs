using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Authentication;

namespace inmobiliaria.Controllers
{
    public class PanelController : Controller
    {
        private readonly UsuarioDao _usuarioDao;
        private readonly IWebHostEnvironment _env;

        public PanelController(IConfiguration config, IWebHostEnvironment env)
        {
            _usuarioDao = new UsuarioDao(config.GetConnectionString("MySqlConnection")!);
            _env = env;
        }

        // GET: /panel
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /panel/perfil
        [Authorize]
        public IActionResult Perfil()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
                return RedirectToAction("Login", "Autenticacion");

            var idClaim = User.FindFirst("Id");
            if (idClaim == null)
                return RedirectToAction("Login", "Autenticacion");

            var idUsuario = int.Parse(idClaim.Value);
            var usuario = _usuarioDao.ObtenerPorId(idUsuario);
            return View(usuario);
        }

        // POST: /panel/perfil
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Perfil(string? NuevaContrasena, string? ConfirmarContrasena, IFormFile? AvatarFile)
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
                return RedirectToAction("Login", "Autenticacion");

            var idClaim = User.FindFirst("Id");
            if (idClaim == null)
                return RedirectToAction("Login", "Autenticacion");

            var idUsuario = int.Parse(idClaim.Value);
            var usuario = _usuarioDao.ObtenerPorId(idUsuario);
            if (usuario == null) return NotFound();

            bool cambioContrasena = false;

            // Actualizar avatar si se subió archivo
            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                // TODO!: Eliminar el avatar viejo del disco!
                // Eliminar archivos viejos de avatar del usuario
                var imgDir = Path.Combine(_env.WebRootPath, "img");
                var avatarBase = $"avatar_{idUsuario}";
                var archivosViejos = Directory.GetFiles(imgDir, $"{avatarBase}.*");
                foreach (var archivo in archivosViejos)
                {
                    try { System.IO.File.Delete(archivo); } catch { /* Ignorar errores de borrado */ }
                }
                var ext = Path.GetExtension(AvatarFile.FileName);
                var fileName = $"avatar_{idUsuario}{ext}";
                var path = Path.Combine(_env.WebRootPath, "img", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    AvatarFile.CopyTo(stream);
                }
                usuario.Avatar = $"/img/{fileName}";

                // Actualizar el claim del avatar en la sesión (esto lo hizo la IA porque no tenía ni idea de cómo implementarlo)
                var claims = User.Claims.ToList();
                var identity = (System.Security.Claims.ClaimsIdentity)User.Identity!;
                // Eliminar el claim viejo de Avatar
                var avatarClaim = claims.FirstOrDefault(c => c.Type == "Avatar");
                if (avatarClaim != null)
                    identity.RemoveClaim(avatarClaim);
                // Agregar el nuevo claim de Avatar
                identity.AddClaim(new System.Security.Claims.Claim("Avatar", usuario.Avatar ?? ""));
                // Re-autenticar el usuario para actualizar los claims
                await HttpContext.SignInAsync(
                    "authCookie",
                    new System.Security.Claims.ClaimsPrincipal(identity)
                );
                // Fin de la actualización del claim del avatar
            }

            // Actualizar contraseña si se ingresó y coincide
            if (!string.IsNullOrEmpty(NuevaContrasena))
            {
                if (NuevaContrasena == ConfirmarContrasena)
                {
                    _usuarioDao.ActualizarContrasena(idUsuario, NuevaContrasena);
                    cambioContrasena = true;
                }
                else
                {
                    ModelState.AddModelError("NuevaContrasena", "Las contraseñas no coinciden.");
                }
            }

            // Actualizar avatar en BD
            _usuarioDao.ActualizarAvatar(idUsuario, usuario.Avatar);

            // Recargar usuario actualizado
            usuario = _usuarioDao.ObtenerPorId(idUsuario);

            if (cambioContrasena)
            {
                ViewBag.Mensaje = "Contraseña actualizada correctamente. Serás redirigido al login en 3 segundos.";
                ViewBag.LogoutEn3s = true;
            }
            else
            {
                ViewBag.Mensaje = "Perfil actualizado correctamente.";
                ViewBag.LogoutEn3s = false;
            }
            return View(usuario);
        }
    }
}
