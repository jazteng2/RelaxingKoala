using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Orders;

namespace RelaxingKoala.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrderRepository _orderRepository;
        private readonly MenuItemRepository _menuItemRepository;
        private readonly UserRepository _userRepository;
        private readonly TableRepository _tableRepository;

        public OrdersController(MySqlDataSource dataSource)
        {
            _orderRepository = new OrderRepository(dataSource);
            _menuItemRepository = new MenuItemRepository(dataSource);
            _userRepository = new UserRepository(dataSource);
            _tableRepository = new TableRepository(dataSource);
        }

        public IActionResult Index()
        {
            var orders = _orderRepository.GetAll();
            return View(orders);
        }

        public IActionResult Create()
        {
            ViewBag.OrderTypes = new SelectList(Enum.GetValues(typeof(OrderType)).Cast<OrderType>().Select(t => new SelectListItem
            {
                Value = ((int)t).ToString(),
                Text = t.ToString()
            }).ToList(), "Value", "Text");

            ViewBag.OrderStates = new SelectList(Enum.GetValues(typeof(OrderState)).Cast<OrderState>().Select(s => new SelectListItem
            {
                Value = ((int)s).ToString(),
                Text = s.ToString()
            }).ToList(), "Value", "Text");

            ViewBag.MenuItems = _menuItemRepository.GetAll().Select(m => new
            {
                Value = m.Id.ToString(),
                Text = m.Name,
                Cost = m.Cost
            }).ToList();

            ViewBag.Users = new SelectList(_userRepository.GetAll().Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.FirstName + " " + u.LastName
            }).ToList(), "Value", "Text");

            ViewBag.Tables = _tableRepository.GetAvailableTables();

            return View(new OrderViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("UserId,OrderTypeId,OrderStateId,MenuItems,Tables")] OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var order = new DineInOrder
                {
                    Id = Guid.NewGuid(),
                    UserId = model.UserId,
                    State = model.OrderStateId,
                    Type = model.OrderTypeId,
                    MenuItems = _menuItemRepository.GetByIds(model.MenuItems)
                };

                if (model.OrderTypeId == OrderType.DineIn)
                {
                    order.Tables = model.Tables.Select(t => new Table { Id = t }).ToList();
                }

                order.RecalculateCost();
                _orderRepository.Insert(order);
                return RedirectToAction(nameof(MyOrders), new { userId = model.UserId });
            }

            ViewBag.OrderTypes = new SelectList(Enum.GetValues(typeof(OrderType)).Cast<OrderType>().Select(t => new SelectListItem
            {
                Value = ((int)t).ToString(),
                Text = t.ToString()
            }).ToList(), "Value", "Text");

            ViewBag.OrderStates = new SelectList(Enum.GetValues(typeof(OrderState)).Cast<OrderState>().Select(s => new SelectListItem
            {
                Value = ((int)s).ToString(),
                Text = s.ToString()
            }).ToList(), "Value", "Text");

            ViewBag.MenuItems = _menuItemRepository.GetAll().Select(m => new
            {
                Value = m.Id.ToString(),
                Text = m.Name,
                Cost = m.Cost
            }).ToList();

            ViewBag.Users = new SelectList(_userRepository.GetAll().Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.FirstName + " " + u.LastName
            }).ToList(), "Value", "Text");

            ViewBag.Tables = _tableRepository.GetAvailableTables();

            return View(model);
        }

        [HttpPost]
        public IActionResult CalculateCost(int[] menuItemIds)
        {
            var menuItems = _menuItemRepository.GetByIds(menuItemIds);
            int totalCost = menuItems.Sum(m => m.Cost);
            return Json(new { cost = totalCost });
        }

        public IActionResult MyOrders(Guid userId)
        {
            var orders = _orderRepository.GetAllByUserId(userId);
            return View(orders);
        }
    }
}
    