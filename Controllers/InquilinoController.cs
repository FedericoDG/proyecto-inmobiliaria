using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using inmobiliaria.Repositories;

namespace inmobiliaria.Controllers
{
    [Route("panel/inquilinos")]
    public class InquilinoController(IConfiguration config) : Controller
    {
        private readonly InquilinoDao _inquilinoDao = new(config.GetConnectionString("MySqlConnection")!);

        // GET: /panel/inquilinos
        // [HttpGet("")]
        // public IActionResult Index()
        // {
        //     var inquilinos = _inquilinoDao.ObtenerTodos();
        //     return View(inquilinos);
        // }
        // GET: /panel/inquilinos?page=1&pageSize=5
        [HttpGet("")]
        public IActionResult Index(int page = 1, int pageSize = 2)
        {
            var total = _inquilinoDao.ContarInquilinos();
            var inquilinos = _inquilinoDao.ObtenerPaginados(page, pageSize);

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = total;
            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

            return View(inquilinos);
        }


        // GET: /panel/inquilinos/crear
        [HttpGet("crear")]
        public IActionResult Crear()
        {
            return View();
        }

        // POST: /panel/inquilinos/crear
        [HttpPost("crear")]
        public IActionResult Crear(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                _inquilinoDao.CrearInquilino(inquilino);
                TempData["Mensaje"] = "Inquilino creado correctamente.";
                return RedirectToAction("Index");
            }
            return View(inquilino);
        }

        // GET: /panel/inquilinos/editar/{id}
        [HttpGet("editar/{id}")]
        public IActionResult Editar(int id)
        {
            var inquilino = _inquilinoDao.ObtenerPorId(id);
            if (inquilino == null)
                return NotFound();
            return View(inquilino);
        }

        // POST: /panel/inquilinos/editar/{id}
        [HttpPost("editar/{id}")]
        public IActionResult Editar(int id, Inquilino inquilino)
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

        // POST: /panel/inquilinos/eliminar/{id}
        [HttpPost("eliminar/{id}")]
        public IActionResult EliminarConfirmado(int id)
        {
            _inquilinoDao.EliminarInquilino(id);
            return RedirectToAction("Index");
        }
    }
}
