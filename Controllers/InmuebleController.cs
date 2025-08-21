using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;

namespace inmobiliaria.Controllers
{
  [Route("panel/inmuebles")]
  public class InmuebleController : Controller
  {
    private readonly InmuebleDao _inmuebleDao;
    private readonly PropietarioDao _propietarioDao;
    private readonly TipoInmuebleDao _tipoInmuebleDao;

    public InmuebleController(IConfiguration config)
    {
      _inmuebleDao = new InmuebleDao(config.GetConnectionString("MySqlConnection")!);
      _propietarioDao = new PropietarioDao(config.GetConnectionString("MySqlConnection")!);
      _tipoInmuebleDao = new TipoInmuebleDao(config.GetConnectionString("MySqlConnection")!);
    }

    // GET: /panel/inmuebles
    [HttpGet("")]
    public IActionResult Index()
    {
      var inmuebles = _inmuebleDao.ObtenerTodos();
      return View(inmuebles);
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
  }
}
