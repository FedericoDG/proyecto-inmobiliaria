using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using inmobiliaria.Config;
using Microsoft.AspNetCore.Authorization;

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

    // GET: /panel/inmuebles
    public IActionResult Index(int page = 1, int pageSize = PaginacionConfig.PageSizeDefault, string? estado = null, int? propietarioId = null)
    {
      try
      {
        // Leer fechas del query string
        var fechaInicioStr = Request.Query["fechaInicio"].ToString();
        var fechaFinStr = Request.Query["fechaFin"].ToString();
        DateTime? fechaInicio = null;
        DateTime? fechaFin = null;
        if (!string.IsNullOrEmpty(fechaInicioStr))
          fechaInicio = DateTime.Parse(fechaInicioStr);
        if (!string.IsNullOrEmpty(fechaFinStr))
          fechaFin = DateTime.Parse(fechaFinStr);

        var inmuebles = _inmuebleDao.ObtenerFiltrados(page, pageSize, propietarioId, estado, fechaInicio, fechaFin);
        var propietarios = _propietarioDao.ObtenerTodos();
        var tipos = _tipoInmuebleDao.ObtenerTodos();

        // Contar total filtrado
        int total = _inmuebleDao.ContarFiltrados(propietarioId, estado, fechaInicio, fechaFin);
        int totalPages = (int)Math.Ceiling((double)total / pageSize);
        ViewBag.Propietarios = propietarios;
        ViewBag.TiposInmueble = tipos;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = totalPages;
        ViewBag.EstadoSeleccionado = estado;
        ViewBag.PropietarioId = propietarioId;
        return View(inmuebles);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Index] Error: {ex.Message}");
        return View(new List<Inmueble>());
      }
    }

    // GET: /panel/inmuebles/obtener-precio/{id}
    [HttpGet("obtener-precio/{id}")]
    public IActionResult ObtenerPrecio(int id)
    {
      try
      {
        var inmueble = _inmuebleDao.ObtenerPorId(id);
        if (inmueble == null)
          return NotFound();
        return Json(new { precio = inmueble.Precio });
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[ObtenerPrecio] Error: {ex.Message}");
        return NotFound();
      }
    }

    // GET: /panel/inmuebles/crear
    [HttpGet("crear")]
    public IActionResult Crear()
    {
      try
      {
        var propietarios = _propietarioDao.ObtenerTodos();
        var tipos = _tipoInmuebleDao.ObtenerTodos();
        ViewBag.Propietarios = propietarios;
        ViewBag.TiposInmueble = tipos;
        return View();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Crear GET] Error: {ex.Message}");
        return View();
      }
    }

    // POST: /panel/inmuebles/crear
    [HttpPost("crear")]
    public IActionResult Crear(Inmueble inmueble)
    {
      try
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
      catch (Exception ex)
      {
        Console.WriteLine($"[Crear POST] Error: {ex.Message}");
        return View(inmueble);
      }
    }

    // GET: /panel/inmuebles/editar/{id}
    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
      try
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
      catch (Exception ex)
      {
        Console.WriteLine($"[Editar GET] Error: {ex.Message}");
        return NotFound();
      }
    }

    // POST: /panel/inmuebles/editar/{id}
    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, Inmueble inmueble)
    {
      try
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
      catch (Exception ex)
      {
        Console.WriteLine($"[Editar POST] Error: {ex.Message}");
        return View(inmueble);
      }
    }

    // POST: /panel/inmuebles/eliminar/{id}
    [Authorize(Roles = "administrador")]
    [HttpPost("eliminar/{id}")]
    public IActionResult EliminarConfirmado(int id)
    {
      try
      {
        _inmuebleDao.EliminarInmueble(id);
        return RedirectToAction("Index");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Eliminar] Error: {ex.Message}");
        return RedirectToAction("Index");
      }
    }

    // GET: /panel/inmuebles/buscar-disponibles
    [HttpGet("buscar-disponibles")]
    public IActionResult BuscarDisponibles(int? idTipo, string uso, int? ambientes, decimal? precio, string fechaInicio, string fechaFin)
    {
      try
      {
        DateTime? inicio = null;
        DateTime? fin = null;
        if (!string.IsNullOrEmpty(fechaInicio))
          inicio = DateTime.Parse(fechaInicio);
        if (!string.IsNullOrEmpty(fechaFin))
          fin = DateTime.Parse(fechaFin);
        var lista = _inmuebleDao.BuscarDisponibles(idTipo, uso, ambientes, precio, inicio, fin);
        var resultado = lista.Select(i => new
        {
          idInmueble = i.IdInmueble,
          direccion = i.Direccion,
          idTipo = i.IdTipo,
          ambientes = i.CantidadAmbientes,
          precio = i.Precio
        });
        return Json(resultado);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[BuscarDisponibles] Error: {ex.Message}");
        return Json(new List<object>());
      }
    }
  }
}
