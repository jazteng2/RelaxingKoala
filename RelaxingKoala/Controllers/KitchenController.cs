using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using RelaxingKoala.Models.ViewModels;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.Users;

namespace RelaxingKoala.Controllers
{
    public class KitchenController : Controller
    {
        private readonly MenuItemRepository menuItemRepo;
        private readonly OrderRepository orderRepo;
        private readonly UserRepository userRepo;
        public KitchenController(MySqlDataSource dataSource)
        {
            menuItemRepo = new MenuItemRepository(dataSource);
            orderRepo = new OrderRepository(dataSource);
            userRepo = new UserRepository(dataSource);
        }
        public IActionResult Index(Guid id)
        {
            List<MenuItem> items = menuItemRepo.GetAll();
            List<IOrder> orders = orderRepo.GetAllByState(OrderState.Confirmed);

            // Get associated users by order
            List<User> users = new List<User>();
            foreach (var order in orders)
            {
                userRepo.GetById(order.UserId);

            }

            return View(new KitchenModel()
            {
                menuItems = items,
                orders = orders
            });
        }

        [HttpPost]
        public IActionResult EditMenuItem(int id, int availability)
        {
            Console.WriteLine("{0} {1}", id, availability);
            var menuitem = menuItemRepo.GetById(id);
            if (availability == 0)
            {
                menuitem.Availability = false;
            }
            else if (availability == 1)
            {
                menuitem.Availability = true;
            }else
            {
                return BadRequest();
            }

            bool updated = menuItemRepo.Update(menuitem);
            if (!updated) return BadRequest();

            return View("Index");
        }
    }
}
