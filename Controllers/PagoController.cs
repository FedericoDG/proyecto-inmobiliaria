using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using inmobiliaria.Config;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria.Controllers
{
  [Route("panel/pagos")]
  public class PagoController(IConfiguration config) : Controller
  {
    private readonly PagoDao _pagoDao = new(config.GetConnectionString("MySqlConnection")!);
    private readonly ContratoDao _contratoDao = new(config.GetConnectionString("MySqlConnection")!);
    private readonly InmuebleDao _inmuebleDao = new(config.GetConnectionString("MySqlConnection")!);

    // GET: /panel/pagos?page=1&pageSize=10
    [HttpGet("")]
    public IActionResult Index(int page = 1, int pageSize = PaginacionConfig.PageSizeDefault)
    {
      try
      {
        string? estado = Request.Query["estado"].ToString();
        var total = _pagoDao.ContarPagosPorEstado(estado);
        var pagos = _pagoDao.ObtenerPaginadosPorEstado(page, pageSize, estado);

        // Obtener contratos e inmuebles para mostrar dirección
        var contratos = _contratoDao.ObtenerTodos();
        var inmuebles = _inmuebleDao.ObtenerTodos();

        ViewBag.Contratos = contratos;
        ViewBag.Inmuebles = inmuebles;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Total = total;
        ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
        ViewBag.EstadoSeleccionado = estado;

        return View(pagos);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Index] Error: {ex.Message}");
        return View(new List<Pago>());
      }
    }

    // GET: /panel/pagos/detalle/{id}
    [HttpGet("detalle/{id}")]
    public IActionResult Detalle(int id)
    {
      try
      {
        var pago = _pagoDao.ObtenerPorId(id);
        if (pago == null)
          return NotFound();
        return View(pago);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Detalle] Error: {ex.Message}");
        return NotFound();
      }
    }

    // GET: /panel/pagos/crear
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

    // POST: /panel/pagos/crear
    [HttpPost("crear")]
    public IActionResult Crear(Pago pago)
    {
      try
      {
        // Asignar el usuario logueado como creador
        var idUsuario = int.Parse(User.FindFirst("Id")!.Value);
        pago.IdUsuarioCreador = idUsuario;
        if (ModelState.IsValid)
        {
          _pagoDao.CrearPago(pago);
          TempData["Mensaje"] = "Pago creado correctamente.";
          return RedirectToAction("Index");
        }
        return View(pago);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Crear POST] Error: {ex.Message}");
        return View(pago);
      }
    }

    // GET: /panel/pagos/editar/{id}
    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
      try
      {
        var pago = _pagoDao.ObtenerPorId(id);
        if (pago == null)
          return NotFound();
        return View(pago);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Editar GET] Error: {ex.Message}");
        return NotFound();
      }
    }

    // POST: /panel/pagos/editar/{id}
    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, Pago pago)
    {
      try
      {
        if (ModelState.IsValid)
        {
          var pagoOriginal = _pagoDao.ObtenerPorId(id);
          if (pagoOriginal == null)
            return NotFound();
          pagoOriginal.Detalle = pago.Detalle;
          pagoOriginal.Estado = pago.Estado;
          // Asignar la fecha de pago desde el modelo recibido
          pagoOriginal.FechaPago = pago.FechaPago;
          if (pago.Estado == "anulado")
          {
            pagoOriginal.IdUsuarioAnulador = int.Parse(User.FindFirst("Id")!.Value);
          }
          else
          {
            pagoOriginal.IdUsuarioAnulador = null;
          }
          _pagoDao.ActualizarPago(pagoOriginal);
          TempData["Mensaje"] = "Pago editado correctamente.";
          return RedirectToAction("Detalle", "Contrato", new { id = pagoOriginal.IdContrato });
        }
        return View(pago);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Editar POST] Error: {ex.Message}");
        return View(pago);
      }
    }

    // POST: /panel/pagos/eliminar/{id}
    [HttpPost("eliminar/{id}")]
    public IActionResult EliminarConfirmado(int id)
    {
      try
      {
        _pagoDao.EliminarPago(id);
        return RedirectToAction("Index");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Eliminar] Error: {ex.Message}");
        return RedirectToAction("Index");
      }
    }

    // POST: /panel/pagos/anular/{id}
    [Authorize(Roles = "administrador")]
    [HttpPost("anular/{id}")]
    public IActionResult Anular(int id)
    {
      try
      {
        var pago = _pagoDao.ObtenerPorId(id);
        if (pago == null)
        {
          return NotFound();
        }
        pago.Estado = "anulado";
        pago.IdUsuarioAnulador = int.Parse(User.FindFirst("Id")!.Value);
        _pagoDao.ActualizarEstado(pago.IdPago, pago.IdUsuarioAnulador ?? 0, "anulado");

        return RedirectToAction("Detalle", "Contrato", new { id = pago.IdContrato });
      }
      catch (Exception ex)
      {
        // Manejo de error
        return RedirectToAction("Error", "Error", new { mensaje = ex.Message });
      }
    }

    // GET: /panel/pagos/buscar-contratos-vigentes?dni=...
    [HttpGet("buscar-contratos-vigentes")]
    public IActionResult BuscarContratosVigentes(string dni)
    {
      var contratos = _pagoDao.BuscarContratosVigentesPorDni(dni);
      return Json(contratos);
    }
  }
}
