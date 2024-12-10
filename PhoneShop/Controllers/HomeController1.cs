using Microsoft.AspNetCore.Mvc;

namespace PhoneShop.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
