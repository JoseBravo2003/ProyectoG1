using Microsoft.AspNetCore.Mvc;

namespace Panel_Usuario.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult AboutUs()
        {
            return View();
        }
    }
}
