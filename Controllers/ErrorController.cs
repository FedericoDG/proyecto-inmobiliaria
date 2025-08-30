using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error404")]
        // GET: /panel/error404
        public IActionResult Error404()
        {
            try
            {
                return View("~/Views/Shared/Error404.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error404] Error: {ex.Message}");
                return View("~/Views/Shared/Error404.cshtml");
            }
        }

        // GET: /panel/error
        public IActionResult Error()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Error: {ex.Message}");
                return View();
            }
        }
    }
}
