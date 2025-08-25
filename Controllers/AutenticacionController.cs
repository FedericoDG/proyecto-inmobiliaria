using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace inmobiliaria.Controllers
{
  public class AutenticacionController(IConfiguration config) : Controller
  {
    private readonly UsuarioDao _usuarioDao = new(config.GetConnectionString("MySqlConnection")!);

    // GET: /autenticacion/login
    [HttpGet]
    public IActionResult Login()
    {
      if (User.Identity != null && User.Identity.IsAuthenticated)
      {
        return Redirect("/panel");
      }
      return View();
    }

    // POST: /autenticacion/login
    [HttpPost]
    public async Task<IActionResult> Login(string email, string contrasena)
    {
      var usuario = _usuarioDao.Login(email, contrasena);
      if (usuario != null)
      {
        var claims = new List<Claim>
        {
          new(ClaimTypes.Name, usuario.Email),
          new(ClaimTypes.Role, usuario.Rol),
          new("Id", usuario.IdUsuario.ToString()),
          new(ClaimTypes.GivenName, usuario.Nombre),
          new(ClaimTypes.Surname, usuario.Apellido),
          new("Avatar", usuario.Avatar ?? "")
        };
        var claimsIdentity = new ClaimsIdentity(claims, "authCookie");
        await HttpContext.SignInAsync("authCookie", new ClaimsPrincipal(claimsIdentity));

        return Redirect("/panel");
      }
      ViewBag.Mensaje = "Login inválido";
      return View();
    }

    // POST: /autenticacion/salir
    [HttpPost]
    public async Task<IActionResult> Salir()
    {
      await HttpContext.SignOutAsync("authCookie");
      return RedirectToAction("Login", "Autenticacion");
    }
  }
}
