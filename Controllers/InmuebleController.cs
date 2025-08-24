using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using inmobiliaria.Config;

namespace inmobiliaria.Controllers
{
  [Route("panel/inmuebles")]
  public class InmuebleController(IConfiguration config) : Controller
  {
    private readonly InmuebleDao _inmuebleDao = new(config.GetConnectionString("MySqlConnection")!);
    private readonly PropietarioDao _propietarioDao = new(config.GetConnectionString("MySqlConnection")!);
    private readonly TipoInmuebleDao _tipoInmuebleDao = new(config.GetConnectionString("MySqlConnection")!);

    // GET: /panel/inmuebles
    [HttpGet("")]
    public IActionResult Index(int page = 1, int pageSize = PaginacionConfig.PageSizeDefault)
    {
      var inmuebles = _inmuebleDao.ObtenerPaginados(page, pageSize);
      var propietarios = _propietarioDao.ObtenerTodos();
      var tipos = _tipoInmuebleDao.ObtenerTodos();
      int total = _inmuebleDao.ContarInmuebles();
      int totalPages = (int)Math.Ceiling((double)total / pageSize);
      ViewBag.Propietarios = propietarios;
      ViewBag.TiposInmueble = tipos;
      ViewBag.Page = page;
      ViewBag.PageSize = pageSize;
      ViewBag.TotalPages = totalPages;
      return View(inmuebles);
    }

    // GET: /panel/inmuebles/obtener-precio/{id}
    [HttpGet("obtener-precio/{id}")]
    public IActionResult ObtenerPrecio(int id)
    {
      var inmueble = _inmuebleDao.ObtenerPorId(id);
      if (inmueble == null)
        return NotFound();
      return Json(new { precio = inmueble.Precio });
    }

    // GET: /panel/inmuebles/crear
    [HttpGet("crear")]
    public IActionResult Crear()
    {
      var propietarios = _propietarioDao.ObtenerTodos();
      var tipos = _tipoInmuebleDao.ObtenerTodos();
      ViewBag.Propietarios = propietarios;
      ViewBag.TiposInmueble = tipos;
      return View();
    }

    // POST: /panel/inmuebles/crear
    [HttpPost("crear")]
    public IActionResult Crear(Inmueble inmueble)
    {
      // if (ModelState.IsValid)
      // {

      // }
      if (inmueble.Uso != "residencial" && inmueble.Uso != "comercial")
      {
        ModelState.AddModelError("Uso", "El uso debe ser 'residencial' o 'comercial'.");
      }
      if (inmueble.Estado != "disponible" && inmueble.Estado != "suspendido" && inmueble.Estado != "ocupado")
      {
        ModelState.AddModelError("Estado", "El estado debe ser 'disponible', 'suspendido' u 'ocupado'.");
      }
      var propietarios = _propietarioDao.ObtenerTodos();
      var tipos = _tipoInmuebleDao.ObtenerTodos();
      ViewBag.Propietarios = propietarios;
      ViewBag.TiposInmueble = tipos;
      if (ModelState.IsValid)
      {
        _inmuebleDao.CrearInmueble(inmueble);
        TempData["Mensaje"] = "Inmueble creado correctamente.";
        return RedirectToAction("Index");
      }
      return View(inmueble);
    }

    // GET: /panel/inmuebles/editar/{id}
    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
      var inmueble = _inmuebleDao.ObtenerPorId(id);
      if (inmueble == null)
        return NotFound();
      var propietarios = _propietarioDao.ObtenerTodos();
      var tipos = _tipoInmuebleDao.ObtenerTodos();
      ViewBag.Propietarios = propietarios;
      ViewBag.TiposInmueble = tipos;
      return View(inmueble);
    }

    // POST: /panel/inmuebles/editar/{id}
    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, Inmueble inmueble)
    {
      if (inmueble.Uso != "residencial" && inmueble.Uso != "comercial")
      {
        ModelState.AddModelError("Uso", "El uso debe ser 'residencial' o 'comercial'.");
      }
      if (inmueble.Estado != "disponible" && inmueble.Estado != "suspendido" && inmueble.Estado != "ocupado")
      {
        ModelState.AddModelError("Estado", "El estado debe ser 'disponible', 'suspendido' u 'ocupado'.");
      }
      if (ModelState.IsValid)
      {
        inmueble.IdInmueble = id;
        _inmuebleDao.ActualizarInmueble(inmueble);
        TempData["Mensaje"] = "Inmueble editado correctamente.";
        return RedirectToAction("Index");
      }
      return View(inmueble);
    }

    // POST: /panel/inmuebles/eliminar/{id}
    [HttpPost("eliminar/{id}")]
    public IActionResult EliminarConfirmado(int id)
    {
      _inmuebleDao.EliminarInmueble(id);
      return RedirectToAction("Index");
    }

    [HttpGet("buscar-por-direccion")]
    public IActionResult BuscarPorDireccion(string direccion)
    {
      var inmuebles = _inmuebleDao.BuscarPorDireccion(direccion);
      var resultado = inmuebles.Select(i => new { idInmueble = i.IdInmueble, direccion = i.Direccion }).ToList();
      return Json(resultado);
    }
  }
}
