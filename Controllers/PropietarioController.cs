using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using inmobiliaria.Config;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria.Controllers
{
  [Route("panel/propietarios")]
  public class PropietarioController(IConfiguration config) : Controller
  {
    private readonly PropietarioDao _propietarioDao = new(config.GetConnectionString("MySqlConnection")!);

    // GET: /panel/propietarios?page=1&pageSize=10
    [HttpGet("")]
    public IActionResult Index(int page = 1, int pageSize = PaginacionConfig.PageSizeDefault)
    {
      try
      {
        string dni = Request.Query["dni"].ToString();
        List<Propietario> propietarios;
        int total;
        if (!string.IsNullOrEmpty(dni))
        {
          propietarios = _propietarioDao.BuscarPorDni(dni, page, pageSize);
          // Para paginación correcta, contar todos los que matchean
          total = _propietarioDao.BuscarPorDni(dni, 1, int.MaxValue).Count;
        }
        else
        {
          total = _propietarioDao.ContarPropietarios();
          propietarios = _propietarioDao.ObtenerPaginados(page, pageSize);
        }

        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Total = total;
        ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
        ViewBag.DniFiltro = dni;

        return View(propietarios);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Index] Error: {ex.Message}");
        return View(new List<Propietario>());
      }
    }

    // GET: /panel/propietarios/buscar-por-dni
    [HttpGet("buscar-por-dni")]
    public IActionResult BuscarPorDni(string dni)
    {
      try
      {
        var propietarios = _propietarioDao.BuscarPorDni(dni);
        var resultado = propietarios.Select(p => new { idPropietario = p.IdPropietario, nombre = p.Nombre, apellido = p.Apellido, dni = p.Dni }).ToList();
        return Json(resultado);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[BuscarPorDni] Error: {ex.Message}");
        return Json(new List<object>());
      }
    }

    // GET: /panel/propietarios/crear
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

    // POST: /panel/propietarios/crear
    [HttpPost("crear")]
    public IActionResult Crear(Propietario propietario)
    {
      try
      {
        if (ModelState.IsValid)
        {
          _propietarioDao.CrearPropietario(propietario);
          TempData["Mensaje"] = "Propietario creado correctamente.";
          return RedirectToAction("Index");
        }
        return View(propietario);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Crear POST] Error: {ex.Message}");
        return View(propietario);
      }
    }

    // GET: /panel/propietarios/editar/{id}
    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
      try
      {
        var propietario = _propietarioDao.ObtenerPorId(id);
        if (propietario == null)
          return NotFound();
        return View(propietario);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Editar GET] Error: {ex.Message}");
        return NotFound();
      }
    }

    // POST: /panel/propietarios/editar/{id}
    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, Propietario propietario)
    {
      try
      {
        if (ModelState.IsValid)
        {
          propietario.IdPropietario = id;
          _propietarioDao.ActualizarPropietario(propietario);
          TempData["Mensaje"] = "Propietario editado correctamente.";
          return RedirectToAction("Index");
        }
        return View(propietario);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Editar POST] Error: {ex.Message}");
        return View(propietario);
      }
    }

    // POST: /panel/propietarios/eliminar/{id}
    [Authorize(Roles = "administrador")]
    [HttpPost("eliminar/{id}")]
    public IActionResult EliminarConfirmado(int id)
    {
      _propietarioDao.EliminarPropietario(id);
      return RedirectToAction("Index");
    }
  }
}
