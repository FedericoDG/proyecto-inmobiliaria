using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;

namespace inmobiliaria.Controllers
{
  [Route("panel/contratos")]
  public class ContratoController : Controller
  {
    private readonly ContratoDao _contratoDao;
    public ContratoController(IConfiguration config)
    {
      _contratoDao = new ContratoDao(config.GetConnectionString("MySqlConnection")!);
    }

    // GET: /panel/contratos?page=1&pageSize=10
    [HttpGet("")]
    public IActionResult Index(int page = 1, int pageSize = 2)
    {
      var total = _contratoDao.ContarContratos();
      var contratos = _contratoDao.ObtenerPaginados(page, pageSize);
      ViewBag.Page = page;
      ViewBag.PageSize = pageSize;
      ViewBag.Total = total;
      ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
      // Para mostrar datos relacionados en la vista
      ViewBag.Inquilinos = _contratoDao.ObtenerInquilinos();
      ViewBag.Inmuebles = _contratoDao.ObtenerInmuebles();
      ViewBag.Usuarios = _contratoDao.ObtenerUsuarios();
      return View(contratos);
    }

    [HttpGet("detalle/{id}")]
    public IActionResult Detalle(int id)
    {
      var contrato = _contratoDao.ObtenerPorId(id);
      if (contrato == null)
        return NotFound();
      // Obtener datos relacionados
      var inquilino = _contratoDao.ObtenerInquilinos().FirstOrDefault(i => i.IdInquilino == contrato.IdInquilino);
      var inmueble = _contratoDao.ObtenerInmuebles().FirstOrDefault(i => i.IdInmueble == contrato.IdInmueble);
      var usuarios = _contratoDao.ObtenerUsuarios();
      var usuarioCreador = usuarios.FirstOrDefault(u => u.IdUsuario == contrato.IdUsuarioCreador);
      var usuarioFinalizador = contrato.IdUsuarioFinalizador != null ? usuarios.FirstOrDefault(u => u.IdUsuario == contrato.IdUsuarioFinalizador) : null;
      ViewBag.Inquilino = inquilino != null ? $"{inquilino.Nombre} {inquilino.Apellido} ({inquilino.Dni})" : contrato.IdInquilino.ToString();
      ViewBag.Inmueble = inmueble != null ? $"{inmueble.Direccion} ({inmueble.TipoNombre})" : contrato.IdInmueble.ToString();
      ViewBag.UsuarioCreador = usuarioCreador != null ? $"{usuarioCreador.Nombre} {usuarioCreador.Apellido} ({usuarioCreador.Email})" : contrato.IdUsuarioCreador.ToString();
      ViewBag.UsuarioFinalizador = usuarioFinalizador != null ? $"{usuarioFinalizador.Nombre} {usuarioFinalizador.Apellido} ({usuarioFinalizador.Email})" : "-";
      return View("Detalle", contrato);
    }

    [HttpGet("crear")]
    public IActionResult Crear()
    {
      ViewBag.Inquilinos = _contratoDao.ObtenerInquilinos();
      ViewBag.Inmuebles = _contratoDao.ObtenerInmuebles();
      ViewBag.Usuarios = _contratoDao.ObtenerUsuarios();
      return View();
    }

    [HttpPost("crear")]
    public IActionResult Crear(Contrato contrato)
    {
      // Asignar el usuario logueado como creador
      var idUsuario = int.Parse(User.FindFirst("Id")!.Value);
      contrato.IdUsuarioCreador = idUsuario;
      // Estado por defecto: vigente
      contrato.Estado = "vigente";
      if (ModelState.IsValid)
      {
        _contratoDao.CrearContrato(contrato);
        TempData["Mensaje"] = "Contrato creado correctamente.";
        return RedirectToAction("Index");
      }

      // TODO: Cuando se crea un crontrato el inmueble pasa a ocupado
      ViewBag.Inquilinos = _contratoDao.ObtenerInquilinos();
      ViewBag.Inmuebles = _contratoDao.ObtenerInmuebles();
      // No cargar usuarios, ya no se selecciona
      return View(contrato);
    }

    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
      var contrato = _contratoDao.ObtenerPorId(id);
      if (contrato == null)
        return NotFound();
      ViewBag.Inquilinos = _contratoDao.ObtenerInquilinos();
      ViewBag.Inmuebles = _contratoDao.ObtenerInmuebles();
      // Buscar datos del usuario creador
      var usuarioCreador = _contratoDao.ObtenerUsuarios().FirstOrDefault(u => u.IdUsuario == contrato.IdUsuarioCreador);
      ViewBag.UsuarioCreador = usuarioCreador != null ? $"{usuarioCreador.Nombre} {usuarioCreador.Apellido} ({usuarioCreador.Email})" : contrato.IdUsuarioCreador.ToString();
      return View(contrato);
    }

    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, Contrato contrato)
    {
      if (contrato.Estado != "rescindido")
      {
        contrato.FechaFinAnticipada = null;
      }
      else
      {
        // Asignar el usuario logueado como finalizador
        contrato.IdUsuarioFinalizador = int.Parse(User.FindFirst("Id")!.Value);
      }

      // Si la fecha de finalización anticipada no es nula, forzar estado a 'rescindido'.
      if (contrato.FechaFinAnticipada != null)
      {
        contrato.Estado = "rescindido";
      }

      if (ModelState.IsValid)
      {
        contrato.IdContrato = id;
        _contratoDao.ActualizarContrato(contrato);
        TempData["Mensaje"] = "Contrato editado correctamente.";
        return RedirectToAction("Index");
      }
      ViewBag.Inquilinos = _contratoDao.ObtenerInquilinos();
      ViewBag.Inmuebles = _contratoDao.ObtenerInmuebles();
      ViewBag.Usuarios = _contratoDao.ObtenerUsuarios();
      return View(contrato);
    }

    [HttpPost("eliminar/{id}")]
    public IActionResult EliminarConfirmado(int id)
    {
      _contratoDao.EliminarContrato(id);
      return RedirectToAction("Index");
    }
  }
}
