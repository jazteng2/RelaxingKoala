using Microsoft.AspNetCore.Mvc;

namespace RelaxingKoala.Controllers
{
    public class StaffController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
