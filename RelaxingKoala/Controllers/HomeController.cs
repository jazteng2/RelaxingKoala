using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Users;
using System.Diagnostics;

namespace RelaxingKoala.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserRepository userRepo;
        public HomeController(MySqlDataSource dataSource)
        {
            userRepo = new UserRepository(dataSource);
        }

        public IActionResult Index()
        {
            var serializedUser = TempData["User"] as string;
            var userRole = TempData["UserRole"];
            if (serializedUser == null && userRole == null) return View();
            switch (userRole)
            {
                case (int) UserRole.Customer:
                    {
                        var user = JsonConvert.DeserializeObject<Customer>(serializedUser);
                        return View(user);
                    }
                case (int)UserRole.Staff:
                    {
                        var user = JsonConvert.DeserializeObject<Staff>(serializedUser);
                        return View(user);
                    }
                case (int)UserRole.Admin:
                    {
                        var user = JsonConvert.DeserializeObject<Admin>(serializedUser);
                        return View(user);
                    }
                case (int)UserRole.Driver:
                    {
                        var user = JsonConvert.DeserializeObject<Driver>(serializedUser);
                        return View(user);
                    }
                default:
                    return View();
            }
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
