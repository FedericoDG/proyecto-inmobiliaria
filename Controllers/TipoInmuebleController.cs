using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;

namespace inmobiliaria.Controllers
{
  [Route("panel/tipos-inmueble")]
  public class TipoInmuebleController(IConfiguration config) : Controller
  {
    private readonly TipoInmuebleDao _tipoDao = new(config.GetConnectionString("MySqlConnection")!);

    // GET: /panel/tipos-inmueble
    [HttpGet("")]
    public IActionResult Index()
    {
      var tipos = _tipoDao.ObtenerTodos();
      return View(tipos);
    }

    // GET: /panel/tipos-inmueble/crear
    [HttpGet("crear")]
    public IActionResult Crear()
    {
      return View();
    }

    // POST: /panel/tipos-inmueble/crear
    [HttpPost("crear")]
    public IActionResult Crear(TipoInmueble tipo)
    {
      if (ModelState.IsValid)
      {
        _tipoDao.CrearTipo(tipo);
        TempData["Mensaje"] = "Tipo de inmueble creado correctamente.";
        return RedirectToAction("Index");
      }
      return View(tipo);
    }

    // GET: /panel/tipos-inmueble/editar/{id}
    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
      var tipo = _tipoDao.ObtenerPorId(id);
      if (tipo == null)
        return NotFound();
      return View(tipo);
    }

    // POST: /panel/tipos-inmueble/editar/{id}
    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, TipoInmueble tipo)
    {
      if (ModelState.IsValid)
      {
        tipo.IdTipo = id;
        _tipoDao.ActualizarTipo(tipo);
        TempData["Mensaje"] = "Tipo de inmueble editado correctamente.";
        return RedirectToAction("Index");
      }
      return View(tipo);
    }

    // POST: /panel/tipos-inmueble/eliminar/{id}
    [HttpPost("eliminar/{id}")]
    public IActionResult EliminarConfirmado(int id)
    {
      _tipoDao.EliminarTipo(id);
      return RedirectToAction("Index");
    }
  }
}
