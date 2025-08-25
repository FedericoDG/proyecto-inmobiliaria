using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using inmobiliaria.Config;

namespace inmobiliaria.Controllers
{
  [Route("panel/pagos")]
  public class PagoController(IConfiguration config) : Controller
  {
    private readonly PagoDao _pagoDao = new(config.GetConnectionString("MySqlConnection")!);

    // GET: /panel/pagos?page=1&pageSize=10
    [HttpGet("")]
    public IActionResult Index(int page = 1, int pageSize = PaginacionConfig.PageSizeDefault)
    {
      var total = _pagoDao.ContarPagos();
      var pagos = _pagoDao.ObtenerPaginados(page, pageSize);

      ViewBag.Page = page;
      ViewBag.PageSize = pageSize;
      ViewBag.Total = total;
      ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

      return View(pagos);
    }

    // GET: /panel/pagos/detalle/{id}
    [HttpGet("detalle/{id}")]
    public IActionResult Detalle(int id)
    {
      var pago = _pagoDao.ObtenerPorId(id);
      if (pago == null)
        return NotFound();
      return View(pago);
    }

    // GET: /panel/pagos/crear
    [HttpGet("crear")]
    public IActionResult Crear()
    {
      return View();
    }

    // POST: /panel/pagos/crear
    [HttpPost("crear")]
    public IActionResult Crear(Pago pago)
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

    // GET: /panel/pagos/editar/{id}
    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
      var pago = _pagoDao.ObtenerPorId(id);
      if (pago == null)
        return NotFound();
      return View(pago);
    }

    // POST: /panel/pagos/editar/{id}
    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, Pago pago)
    {
      if (ModelState.IsValid)
      {
        pago.IdPago = id;
        _pagoDao.ActualizarPago(pago);
        TempData["Mensaje"] = "Pago editado correctamente.";
        return RedirectToAction("Index");
      }
      return View(pago);
    }

    // POST: /panel/pagos/eliminar/{id}
    [HttpPost("eliminar/{id}")]
    public IActionResult EliminarConfirmado(int id)
    {
      _pagoDao.EliminarPago(id);
      return RedirectToAction("Index");
    }

    // POST: /panel/pagos/anular/{id}
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
  }
}
