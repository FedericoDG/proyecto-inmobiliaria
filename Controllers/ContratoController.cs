using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using inmobiliaria.Config;

namespace inmobiliaria.Controllers
{
  [Route("panel/contratos")]
  public class ContratoController(IConfiguration config) : Controller
  {
    private readonly ContratoDao _contratoDao = new(config.GetConnectionString("MySqlConnection")!);
    private readonly InmuebleDao _inmuebleDao = new(config.GetConnectionString("MySqlConnection")!);
    private readonly PagoDao _pagoDao = new(config.GetConnectionString("MySqlConnection")!);
    private readonly TipoInmuebleDao _tipoInmuebleDao = new(config.GetConnectionString("MySqlConnection")!);

    // GET: /panel/contratos?page=1&pageSize=10
    [HttpGet("")]
    public IActionResult Index(int page = 1, int pageSize = PaginacionConfig.PageSizeDefault, int? inmuebleId = null, string? fechaDesde = null, string? fechaHasta = null, int? diasVencimiento = null, int? diasVencimientoPersonalizado = null)
    {
      try
      {
        int total;
        List<Contrato> contratos;
        DateTime? desde = null;
        DateTime? hasta = null;
        if (!string.IsNullOrEmpty(fechaDesde))
          desde = DateTime.Parse(fechaDesde);
        if (!string.IsNullOrEmpty(fechaHasta))
          hasta = DateTime.Parse(fechaHasta);

        int? plazo = null;
        if (diasVencimientoPersonalizado.HasValue && diasVencimientoPersonalizado.Value > 0)
          plazo = diasVencimientoPersonalizado;
        else if (diasVencimiento.HasValue && diasVencimiento.Value > 0)
          plazo = diasVencimiento;

        bool filtroFechas = desde != null && hasta != null;
        bool filtroPlazo = plazo != null;

        if (filtroFechas)
        {
          // GET: /panel/contratos?page=1&pageSize=10 (filtrar por fechas)
          total = _contratoDao.ContarContratosVigentesPorFechas(desde.Value, hasta.Value); // TODO: pueden ser nulos
          contratos = _contratoDao.ObtenerPaginadosVigentesPorFechas(page, pageSize, desde.Value, hasta.Value);
        }
        else if (filtroPlazo)
        {
          // GET: /panel/contratos?page=1&pageSize=10 (filtrar por vencimiento)
          var fechaLimite = DateTime.Today.AddDays(plazo ?? 0);
          total = _contratoDao.ContarContratosPorVencimiento(fechaLimite);
          contratos = _contratoDao.ObtenerContratosPorVencimiento(page, pageSize, fechaLimite);
        }
        else if (inmuebleId != null)
        {
          // GET: /panel/contratos?page=1&pageSize=10 (filtrar por inmueble)
          total = _contratoDao.ContarContratosPorInmueble(inmuebleId.Value);
          contratos = _contratoDao.ObtenerPaginadosPorInmueble(page, pageSize, inmuebleId);
        }
        else
        {
          total = _contratoDao.ContarContratos();
          contratos = _contratoDao.ObtenerPaginados(page, pageSize);
        }
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Total = total;
        ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize); // valor "techo"
        ViewBag.InmuebleId = inmuebleId;
        ViewBag.FechaDesde = fechaDesde;
        ViewBag.FechaHasta = fechaHasta;
        // Datos relacionados en la vista
        ViewBag.Inquilinos = _contratoDao.ObtenerInquilinos();
        ViewBag.Inmuebles = _contratoDao.ObtenerInmuebles();
        ViewBag.Usuarios = _contratoDao.ObtenerUsuarios();
        ViewBag.DiasVencimiento = diasVencimiento;
        ViewBag.DiasVencimientoPersonalizado = diasVencimientoPersonalizado;
        return View(contratos);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Index] Error: {ex.Message}");
        return View(new List<Contrato>());
      }
    }

    // GET: /panel/contratos/detalle/{id}
    [HttpGet("detalle/{id}")]
    public IActionResult Detalle(int id)
    {
      try
      {
        var contrato = _contratoDao.ObtenerPorId(id);
        if (contrato == null)
          return NotFound();
        // Datos relacionados en la vista
        var inquilino = _contratoDao.ObtenerInquilinos().FirstOrDefault(i => i.IdInquilino == contrato.IdInquilino);
        var inmueble = _contratoDao.ObtenerInmuebles().FirstOrDefault(i => i.IdInmueble == contrato.IdInmueble);
        var usuarios = _contratoDao.ObtenerUsuarios();
        var usuarioCreador = usuarios.FirstOrDefault(u => u.IdUsuario == contrato.IdUsuarioCreador);
        var usuarioFinalizador = contrato.IdUsuarioFinalizador != null ? usuarios.FirstOrDefault(u => u.IdUsuario == contrato.IdUsuarioFinalizador) : null;
        ViewBag.Inquilino = inquilino != null ? $"{inquilino.Nombre} {inquilino.Apellido} ({inquilino.Dni})" : contrato.IdInquilino.ToString();
        ViewBag.Inmueble = inmueble != null ? $"{inmueble.Direccion} ({inmueble.TipoNombre})" : contrato.IdInmueble.ToString();
        ViewBag.UsuarioCreador = usuarioCreador != null ? $"{usuarioCreador.Nombre} {usuarioCreador.Apellido} ({usuarioCreador.Email})" : contrato.IdUsuarioCreador.ToString();
        ViewBag.UsuarioFinalizador = usuarioFinalizador != null ? $"{usuarioFinalizador.Nombre} {usuarioFinalizador.Apellido} ({usuarioFinalizador.Email})" : "-";

        // Estos son los pagos asociados
        var pagos = _pagoDao.ObtenerTodos().Where(p => p.IdContrato == id).ToList();
        ViewBag.Pagos = pagos;

        return View("Detalle", contrato);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Detalle] Error: {ex.Message}");
        return NotFound();
      }
    }

    // GET: /panel/contratos/crear
    [HttpGet("crear")]
    public IActionResult Crear()
    {
      try
      {
        ViewBag.Inquilinos = _contratoDao.ObtenerInquilinos();
        ViewBag.Inmuebles = _contratoDao.ObtenerInmuebles();
        ViewBag.Usuarios = _contratoDao.ObtenerUsuarios();
        ViewBag.TiposInmueble = _tipoInmuebleDao.ObtenerTodos();
        return View();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Crear GET] Error: {ex.Message}");
        return View();
      }
    }

    // POST: /panel/contratos/crear
    [HttpPost("crear")]
    public IActionResult Crear(Contrato contrato)
    {
      try
      {
        // Usar la cantidad de cuotas enviada desde el formulario
        int cantDePagos = 0;
        bool cuotasValidas = false;
        if (Request.Form.ContainsKey("CantidadCuotas"))
        {
          cuotasValidas = int.TryParse(Request.Form["CantidadCuotas"], out cantDePagos) && cantDePagos > 0;
        }
        if (!cuotasValidas)
        {
          ModelState.AddModelError("CantidadCuotas", "La cantidad de cuotas es obligatoria");
        }

        // Asignar el usuario logueado como creador
        var idUsuario = int.Parse(User.FindFirst("Id")!.Value);
        contrato.IdUsuarioCreador = idUsuario;
        // Estado por defecto: vigente
        contrato.Estado = "vigente";
        if (ModelState.IsValid)
        {
          var idContratoCreado = _contratoDao.CrearContrato(contrato);
          var idUsuarioLoagueado = int.Parse(User.FindFirst("Id")!.Value);

          // Crear los pagos pendientes asociados al contrato
          if (contrato.MontoMensual.HasValue)
          {
            _pagoDao.CrearPagosPendientes(idContratoCreado, contrato.FechaInicio!.Value, cantDePagos, contrato.MontoMensual.Value, idUsuarioLoagueado);
          }

          // Actualizar estado del inmueble a 'ocupado' de forma simple
          _inmuebleDao.ActualizarEstado((int)contrato.IdInmueble!, "ocupado");
          TempData["Mensaje"] = "Contrato creado correctamente.";
          return RedirectToAction("Index");
        }

        ViewBag.Inquilinos = _contratoDao.ObtenerInquilinos();
        ViewBag.Inmuebles = _contratoDao.ObtenerInmuebles();
        return View(contrato);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Crear POST] Error: {ex.Message}");
        return View(contrato);
      }
    }

    // GET: /panel/contratos/editar/{id}
    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
      try
      {
        var contrato = _contratoDao.ObtenerPorId(id);
        if (contrato == null)
          return NotFound();
        ViewBag.Inquilinos = _contratoDao.ObtenerInquilinos();
        ViewBag.Inmuebles = _contratoDao.ObtenerInmuebles();
        // Usuario creador
        var usuarioCreador = _contratoDao.ObtenerUsuarios().FirstOrDefault(u => u.IdUsuario == contrato.IdUsuarioCreador);
        ViewBag.UsuarioCreador = usuarioCreador != null ? $"{usuarioCreador.Nombre} {usuarioCreador.Apellido} ({usuarioCreador.Email})" : contrato.IdUsuarioCreador.ToString();
        return View(contrato);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Editar GET] Error: {ex.Message}");
        return NotFound();
      }
    }

    // POST: /panel/contratos/editar/{id}
    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, Contrato contrato)
    {
      try
      {
        if (contrato.Estado == "rescindido")
        {
          // Asignar el usuario logueado como finalizador
          contrato.IdUsuarioFinalizador = int.Parse(User.FindFirst("Id")!.Value);
        }
        else
        {
          contrato.FechaFinAnticipada = null;

        }

        if (contrato.Estado == "rescindido" || contrato.Estado == "finalizado")
        {
          _inmuebleDao.ActualizarEstado((int)contrato.IdInmueble!, "disponible");
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
      catch (Exception ex)
      {
        Console.WriteLine($"[Editar POST] Error: {ex.Message}");
        return View(contrato);
      }
    }

    // POST: /panel/contratos/renovar
    [HttpPost("renovar")]
    public IActionResult Renovar(int IdInquilino, int IdInmueble, DateTime NuevaFechaInicio, DateTime NuevaFechaFin, decimal NuevoMontoMensual)
    {
      try
      {
        var idUsuario = int.Parse(User.FindFirst("Id")!.Value);
        int idContratoAnterior = int.Parse(Request.Form["IdContratoAnterior"]); // TODO: Manejar nulo
        var contratoAnterior = _contratoDao.ObtenerPorId(idContratoAnterior);

        var nuevoContrato = new Contrato
        {
          IdInquilino = IdInquilino,
          IdInmueble = IdInmueble,
          IdUsuarioCreador = idUsuario,
          FechaInicio = NuevaFechaInicio,
          FechaFinOriginal = NuevaFechaFin,
          MontoMensual = NuevoMontoMensual,
          Estado = "vigente"
        };
        if (ModelState.IsValid)
        {
          _contratoDao.CrearContrato(nuevoContrato);
          _inmuebleDao.ActualizarEstado(IdInmueble, "ocupado");
          // Finalizar el contrato anterior si existe
          if (contratoAnterior != null)
          {
            contratoAnterior.Estado = "finalizado";
            _contratoDao.ActualizarContrato(contratoAnterior);
          }
          TempData["Mensaje"] = "Contrato renovado correctamente.";
          return RedirectToAction("Index");
        }
        // Si hay error
        TempData["Error"] = "No se pudo renovar el contrato. Verifique los datos.";
        return RedirectToAction("Index");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Renovar] Error: {ex.Message}");
        TempData["Error"] = "No se pudo renovar el contrato. Verifique los datos.";
        return RedirectToAction("Index");
      }
    }

    // POST: /panel/contratos/rescindir
    [Authorize(Roles = "administrador")]
    [HttpPost("rescindir")]
    public IActionResult Rescindir(int IdContrato, DateTime FechaFinAnticipada, decimal MontoMensualRescision, decimal Multa)
    {
      try
      {
        var contrato = _contratoDao.ObtenerPorId(IdContrato);
        if (contrato == null)
          return NotFound();

        // Cálculo de multa
        if (contrato.FechaInicio.HasValue && contrato.FechaFinOriginal.HasValue)
        {
          var inicio = contrato.FechaInicio.Value;
          var finOriginal = contrato.FechaFinOriginal.Value;
          var finAnticipada = FechaFinAnticipada;

          // Cálculo de meses transcurridos y total
          int mesesOriginal = (finOriginal.Year - inicio.Year) * 12 + (finOriginal.Month - inicio.Month);
          int mesesAnticipada = (finAnticipada.Year - inicio.Year) * 12 + (finAnticipada.Month - inicio.Month);
          int mitadContrato = mesesOriginal / 2;

          // Cálculo de multa
          decimal multaCalculada = contrato.MontoMensual ?? 0;
          if (mesesAnticipada < mitadContrato)
          {
            multaCalculada *= 2;
          }
          Console.WriteLine($"[Rescindir] Multa calculada en el Controlador: {multaCalculada}. Multa calculada en la vista: {Multa}");

          // if (Math.Abs(multaCalculada - Multa) > 0.01m) // tolerancia de 1 centavo, antes puse multaCalculada != Multa y obviamente eran siempre distintas
          // {
          //   TempData["Error"] = "La multa calculada no coincide. Verifique los datos.";
          //   return RedirectToAction("Index");
          // }
        }
        //

        contrato.Estado = "rescindido";
        contrato.FechaFinAnticipada = FechaFinAnticipada;
        contrato.Multa = Multa;
        contrato.IdUsuarioFinalizador = int.Parse(User.FindFirst("Id")!.Value);
        _contratoDao.RescindirContrato(contrato);

        // Anular pagos pendientes desde la fecha de rescisión
        _pagoDao.CancelarPagosPorContrato(IdContrato, FechaFinAnticipada);

        // Crear un nuevo pago por la multa
        if (Multa > 0)
        {
          var pagoMulta = new Pago
          {
            IdContrato = IdContrato,
            NumeroPago = 0, // Indica que es una multa
            FechaVencimiento = DateTime.Today,
            Importe = Multa,
            Detalle = "Multa por rescisión anticipada",
            Estado = "pendiente",
            IdUsuarioCreador = contrato.IdUsuarioCreador
          };
          _pagoDao.CrearPago(pagoMulta);
        }

        // Inmueble pasa a estado disponible
        if (contrato.IdInmueble != null)
          _inmuebleDao.ActualizarEstado((int)contrato.IdInmueble, "disponible");
        TempData["Mensaje"] = "Contrato rescindido correctamente.";
        return RedirectToAction("Index");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Rescindir] Error: {ex.Message}");
        TempData["Error"] = "No se pudo rescindir el contrato.";
        return RedirectToAction("Index");
      }
    }
  }
}
