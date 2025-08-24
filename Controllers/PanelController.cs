using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliaria.Repositories;

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
        public IActionResult Perfil(string? NuevaContrasena, string? ConfirmarContrasena, IFormFile? AvatarFile)
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
                return RedirectToAction("Login", "Autenticacion");

            var idClaim = User.FindFirst("Id");
            if (idClaim == null)
                return RedirectToAction("Login", "Autenticacion");

            var idUsuario = int.Parse(idClaim.Value);
            var usuario = _usuarioDao.ObtenerPorId(idUsuario);
            if (usuario == null) return NotFound();

            // Actualizar avatar si se subió archivo
            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var ext = Path.GetExtension(AvatarFile.FileName);
                var fileName = $"avatar_{idUsuario}{ext}";
                var path = Path.Combine(_env.WebRootPath, "img", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    AvatarFile.CopyTo(stream);
                }
                usuario.Avatar = $"/img/{fileName}";
            }

            // Actualizar contraseña si se ingresó y coincide
            if (!string.IsNullOrEmpty(NuevaContrasena))
            {
                if (NuevaContrasena == ConfirmarContrasena)
                {
                    _usuarioDao.ActualizarContrasena(idUsuario, NuevaContrasena);
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
            ViewBag.Mensaje = "Perfil actualizado correctamente.";
            return View(usuario);
        }
    }
}
