using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.ViewModels;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace RelaxingKoala.Controllers
{
    [Authorize]
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
            List<MenuItem> items = menuItemRepo.GetAll();

            // Get Confirmed, Preparing, Ready orders
            List<IOrder> confirmedOrders = orderRepo.GetAllByState(OrderState.Confirmed);
            List<IOrder> preparingOrders = orderRepo.GetAllByState(OrderState.Preparing);
            List<IOrder> readyOrders = orderRepo.GetAllByState(OrderState.Ready);
            List<IOrder> orders = confirmedOrders.Concat(preparingOrders).Concat(readyOrders).ToList();
            List<IOrder> updatedOrders = orderRepo.PopulateAssociations(orders);
            return View(new KitchenViewModel()
            {
                menuItems = items,
                orders = updatedOrders,
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

        [HttpPost]
        public IActionResult EditOrder(Guid id, int state)
        {
            Console.WriteLine("{0} {1}", id, state);
            var order = orderRepo.GetById(id);
            var orderState = orderRepo.GetOrderState(state);
            order.State = orderState;
            bool updated = orderRepo.Update(order);
            if (!updated) return BadRequest();
            return View("Index");
        }
    }
}
