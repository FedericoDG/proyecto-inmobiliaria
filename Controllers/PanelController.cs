using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Repositories;
using Microsoft.Extensions.Configuration;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria.Controllers
{
    public class PanelController(IConfiguration config) : Controller
    {
        private readonly PropietarioDao _propietarioDao = new(config.GetConnectionString("MySqlConnection")!);

        // GET: /panel
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
