using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Users;
using System.Diagnostics;
using System.Security.Claims;

namespace RelaxingKoala.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserRepository userRepo;
        public HomeController(MySqlDataSource dataSource)
        {
            userRepo = new UserRepository(dataSource);
        }

        public IActionResult Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return View();
            Console.WriteLine(userId);
            User user = userRepo.GetById(new Guid(userId));
            user.Password = string.Empty;
            return View(user);
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
