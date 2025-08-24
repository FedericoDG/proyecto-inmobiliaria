using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using inmobiliaria.Config;

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
      var total = _propietarioDao.ContarPropietarios();
      var propietarios = _propietarioDao.ObtenerPaginados(page, pageSize);

      ViewBag.Page = page;
      ViewBag.PageSize = pageSize;
      ViewBag.Total = total;
      ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

      return View(propietarios);
    }

    [HttpGet("buscar-por-dni")]
    public IActionResult BuscarPorDni(string dni)
    {
      var propietarios = _propietarioDao.BuscarPorDni(dni);
      var resultado = propietarios.Select(p => new { idPropietario = p.IdPropietario, nombre = p.Nombre, apellido = p.Apellido, dni = p.Dni }).ToList();
      return Json(resultado);
    }

    // GET: /panel/propietarios
    // [HttpGet("")]
    // public IActionResult Index()
    // {
    //   var propietarios = _propietarioDao.ObtenerTodos();
    //   return View(propietarios);
    // }

    // GET: /panel/propietarios/crear
    [HttpGet("crear")]
    public IActionResult Crear()
    {
      return View();
    }

    // POST: /panel/propietarios/crear
    [HttpPost("crear")]
    public IActionResult Crear(Propietario propietario)
    {
      if (ModelState.IsValid)
      {
        _propietarioDao.CrearPropietario(propietario);
        TempData["Mensaje"] = "Propietario creado correctamente.";
        return RedirectToAction("Index");
      }
      return View(propietario);
    }

    // GET: /panel/propietarios/editar/{id}
    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
      var propietario = _propietarioDao.ObtenerPorId(id);
      if (propietario == null)
        return NotFound();
      return View(propietario);
    }

    // POST: /panel/propietarios/editar/{id}
    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, Propietario propietario)
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

    // POST: /panel/propietarios/eliminar/{id}
    [HttpPost("eliminar/{id}")]
    public IActionResult EliminarConfirmado(int id)
    {
      _propietarioDao.EliminarPropietario(id);
      return RedirectToAction("Index");
    }
  }
}
