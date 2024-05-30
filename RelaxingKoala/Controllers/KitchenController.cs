using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using RelaxingKoala.Data;

namespace RelaxingKoala.Controllers
{
    public class KitchenController : Controller
    {
        private readonly MenuItemRepository menuItemRepo;
        private readonly OrderRepository orderRepo;
        public KitchenController(MySqlDataSource dataSource)
        {
            menuItemRepo = new MenuItemRepository(dataSource);
            orderRepo = new OrderRepository(dataSource);
        }
        public IActionResult Index()
        {
            ViewBag.MenuItems = menuItemRepo.GetAll();
            return View();
        }

        public IActionResult EditMenuItem(int id, bool availability)
        {
            Console.WriteLine("{0} {1}", id, availability);
            var menuitem = menuItemRepo.GetById(id);
            menuitem.Availability = availability;
            menuItemRepo.Update(menuitem);
            ViewBag.MenuItems = menuItemRepo.GetAll();
            return View(); 
        }
    }
}
