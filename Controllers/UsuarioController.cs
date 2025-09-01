using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria.Controllers
{
  [Route("panel/usuarios")]
  [Authorize(Roles = "administrador")]
  public class UsuarioController : Controller
  {
    private readonly UsuarioDao _usuarioDao;
    private readonly IConfiguration _config;

    public UsuarioController(IConfiguration config)
    {
      _config = config;
      _usuarioDao = new UsuarioDao(_config.GetConnectionString("MySqlConnection")!);
    }


    // GET: /panel/usuarios
    [HttpGet("")]
    public IActionResult Index()
    {
      try
      {
        var usuarios = _usuarioDao.ObtenerTodos();
        return View(usuarios);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Index] Error: {ex.Message}");
        return View(new List<Usuario>());
      }
    }

    // GET: /panel/usuarios/crear
    [HttpGet("crear")]
    public IActionResult Crear()
    {
      try
      {
        return View();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Crear GET] Error: {ex.Message}");
        return View();
      }
    }

    // POST: /panel/usuarios/crear
    [HttpPost("crear")]
    public IActionResult Crear(Usuario usuario)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(usuario.Contrasena))
        {
          // Elimina cualquier error por defecto (Lo hizo la Ia, estaba trabadísimo acá!)
          if (ModelState.ContainsKey(nameof(usuario.Contrasena)))
            ModelState[nameof(usuario.Contrasena)]?.Errors.Clear();
          ModelState.Remove("Contrasena");
          // TODO: Agregar validación personalizada
          ModelState.AddModelError("Contrasena", "La contraseña es obligatoria.");
        }
        if (ModelState.IsValid)
        {
          // El estado (Activo) lo elige el usuario desde el formulario (así puedo recuperarlos en caso de ser "borrados")
          _usuarioDao.CrearUsuario(usuario);
          TempData["Mensaje"] = "Usuario creado correctamente.";
          return RedirectToAction("Index");
        }
        return View(usuario);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Crear POST] Error: {ex.Message}");
        return View(usuario);
      }
    }

    // GET: /panel/usuarios/editar/{id}
    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
      try
      {
        var usuario = _usuarioDao.ObtenerPorId(id);
        if (usuario == null)
          return NotFound();
        return View(usuario);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Editar GET] Error: {ex.Message}");
        return NotFound();
      }
    }

    // POST: /panel/usuarios/editar/{id}
    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, Usuario usuario)
    {
      try
      {
        // Si el campo Contrasena está vacío, elimina cualquier error de validación para permitir edición opcional
        if (string.IsNullOrWhiteSpace(usuario.Contrasena))
        {
          if (ModelState.ContainsKey(nameof(usuario.Contrasena)))
            ModelState[nameof(usuario.Contrasena)]?.Errors.Clear();
          // Elimina cualquier error residual manualmente
          ModelState.Remove("Contrasena");
        }
        if (ModelState.IsValid)
        {
          usuario.IdUsuario = id;
          // Si se especifica una nueva contraseña hay que actualizarla
          if (!string.IsNullOrWhiteSpace(usuario.Contrasena))
          {
            _usuarioDao.ActualizarContrasena(id, usuario.Contrasena);
          }
          _usuarioDao.ActualizarUsuario(usuario);
          TempData["Mensaje"] = "Usuario editado correctamente.";
          return RedirectToAction("Index");
        }
        return View(usuario);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Editar POST] EXCEPCION: {ex.Message}");
        return View(usuario);
      }
    }

    // POST: /panel/usuarios/eliminar/{id}
    [HttpPost("eliminar/{id}")]
    public IActionResult EliminarConfirmado(int id)
    {
      _usuarioDao.EliminarUsuario(id);
      return RedirectToAction("Index");
    }
  }
}
