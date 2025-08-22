using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria.Controllers
{
    public class PanelController() : Controller
    {
        // GET: /panel
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
