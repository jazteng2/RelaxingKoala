using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models.ViewModels;

namespace RelaxingKoala.Controllers
{
    public class MenuController : Controller
    {
        private readonly MenuItemRepository menuItemRepo;
        private readonly UserRepository userRepo;
        public MenuController(MySqlDataSource dataSource)
        {
            menuItemRepo = new MenuItemRepository(dataSource);
            userRepo = new UserRepository(dataSource);
        }
        public IActionResult Index(Guid? userId)
        {
            var items = menuItemRepo.GetAll();
            var viewModel = new MenuViewModel();
            viewModel.MenuItems = items;
            if (userId.HasValue)
            {
                viewModel.User = userRepo.GetById(userId.Value);
            }

            return View(viewModel);
        }
    }
}
