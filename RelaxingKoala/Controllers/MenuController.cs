using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models.ViewModels;
using System.Security.Claims;

namespace RelaxingKoala.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly MenuItemRepository menuItemRepo;
        private readonly UserRepository userRepo;
        public MenuController(MySqlDataSource dataSource)
        {
            menuItemRepo = new MenuItemRepository(dataSource);
            userRepo = new UserRepository(dataSource);
        }
        public IActionResult Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return View();
            Console.WriteLine(userId);

            var viewModel = new MenuViewModel();
            viewModel.MenuItems = menuItemRepo.GetAll();
            viewModel.User = userRepo.GetById(new Guid(userId));

            return View(viewModel);
        }
    }
}
