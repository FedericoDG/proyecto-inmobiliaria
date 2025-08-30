using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;
using inmobiliaria.Config;

namespace inmobiliaria.Controllers
{
    [Route("panel/inquilinos")]
    public class InquilinoController(IConfiguration config) : Controller
    {
        private readonly InquilinoDao _inquilinoDao = new(config.GetConnectionString("MySqlConnection")!);

        // GET: /panel/inquilinos?page=1&pageSize=5
        [HttpGet("")]
        public IActionResult Index(int page = 1, int pageSize = PaginacionConfig.PageSizeDefault)
        {
            try
            {
                var total = _inquilinoDao.ContarInquilinos();
                var inquilinos = _inquilinoDao.ObtenerPaginados(page, pageSize);

                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.Total = total;
                ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

                return View(inquilinos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Index] Error: {ex.Message}");
                return View(new List<Inquilino>());
            }
        }

        // GET: /panel/inquilinos/buscar-por-dni
        [HttpGet("buscar-por-dni")]
        public IActionResult BuscarPorDni(string dni)
        {
            try
            {
                var inquilinos = _inquilinoDao.BuscarPorDni(dni);
                var resultado = inquilinos.Select(i => new { idInquilino = i.IdInquilino, nombre = i.Nombre, apellido = i.Apellido, dni = i.Dni }).ToList();
                return Json(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BuscarPorDni] Error: {ex.Message}");
                return Json(new List<object>());
            }
        }

        // GET: /panel/inquilinos/crear
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

        // POST: /panel/inquilinos/crear
        [HttpPost("crear")]
        public IActionResult Crear(Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _inquilinoDao.CrearInquilino(inquilino);
                    TempData["Mensaje"] = "Inquilino creado correctamente.";
                    return RedirectToAction("Index");
                }
                return View(inquilino);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Crear POST] Error: {ex.Message}");
                return View(inquilino);
            }
        }

        // GET: /panel/inquilinos/editar/{id}
        [HttpGet("editar/{id}")]
        public IActionResult Editar(int id)
        {
            try
            {
                var inquilino = _inquilinoDao.ObtenerPorId(id);
                if (inquilino == null)
                    return NotFound();
                return View(inquilino);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Editar GET] Error: {ex.Message}");
                return NotFound();
            }
        }

        // POST: /panel/inquilinos/editar/{id}
        [HttpPost("editar/{id}")]
        public IActionResult Editar(int id, Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    inquilino.IdInquilino = id;
                    _inquilinoDao.ActualizarInquilino(inquilino);
                    TempData["Mensaje"] = "Inquilino editado correctamente.";
                    return RedirectToAction("Index");
                }
                return View(inquilino);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Editar POST] Error: {ex.Message}");
                return View(inquilino);
            }
        }

        // POST: /panel/inquilinos/eliminar/{id}
        [HttpPost("eliminar/{id}")]
        public IActionResult EliminarConfirmado(int id)
        {
            _inquilinoDao.EliminarInquilino(id);
            return RedirectToAction("Index");
        }
    }
}
