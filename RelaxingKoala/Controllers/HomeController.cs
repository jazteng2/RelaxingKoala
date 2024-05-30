using Microsoft.AspNetCore.Mvc;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Users;
using System.Diagnostics;

namespace RelaxingKoala.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {

        }

        public IActionResult Index()
        {
            if (TempData["UserId"] != null && TempData["UserRole"] != null)
            {
                ViewBag.UserId = TempData["UserId"];
                ViewBag.UserRole = TempData["UserRole"];
            } 
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
