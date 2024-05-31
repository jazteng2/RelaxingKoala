using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.Users;
using RelaxingKoala.Models.ViewModels;
using System.Security.Claims;

namespace RelaxingKoala.Controllers
{
    [Authorize]
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
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            // Check role
            UserRole role = _userRepository.GetById(new Guid(userId)).Role;
            List<IOrder> orders;
            if (role == UserRole.Admin || role == UserRole.Staff)
            {
                orders = _orderRepository.GetAll();
            }
            else
            {
                orders = _orderRepository.GetAllByUserId(new Guid(userId));
            }
            return View(orders);
        }

        public IActionResult Create()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

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

            return View(new OrderViewModel()
            {
                UserId = new Guid(userId)
            });
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

        public IActionResult MyOrders()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");
            var orders = _orderRepository.GetAllByUserId(new Guid(userId));
            return View(orders);
        }

        public IActionResult Edit(Guid id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderViewModel
            {
                UserId = order.UserId,
                OrderTypeId = order.Type,
                OrderStateId = order.State,
                MenuItems = order.MenuItems.Select(mi => mi.Id).ToList(),
                Tables = order is DineInOrder dineInOrder ? dineInOrder.Tables.Select(t => t.Id).ToList() : new List<int>()
            };

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

        // POST: Orders/Edit/5
        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("UserId,OrderTypeId,OrderStateId,MenuItems,Tables")] OrderViewModel model)
        {
            if (id == Guid.Empty || model == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var order = _orderRepository.GetById(id);
                if (order == null)
                {
                    return NotFound();
                }

                order.UserId = model.UserId;
                order.State = model.OrderStateId;
                order.Type = model.OrderTypeId;
                order.MenuItems = _menuItemRepository.GetByIds(model.MenuItems);

                if (model.OrderTypeId == OrderType.DineIn)
                {
                    order.Tables = model.Tables.Select(t => new Table { Id = t }).ToList();
                }

                order.RecalculateCost();
                _orderRepository.Update(order);

                var hardcodedUserId = new Guid("d840d48c-91b8-4a69-97b9-1b90a8ab3acf");
                return RedirectToAction(nameof(MyOrders), new { userId = hardcodedUserId });
            }

            return View(model);
        }

        // GET: Orders/Delete/5
        public IActionResult Delete(Guid id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id, string origin)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }
            _orderRepository.Delete(id);

            if (origin == "index")
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(MyOrders), new { userId = order.UserId });
            }
        }

        [HttpGet]
        public IActionResult PayOrder(Guid orderId)
        {
            Console.WriteLine(orderId);
            return View(new PaymentViewModel() { Order = _orderRepository.GetById(orderId) });

        }
        [HttpPost, ActionName("PayOrder")]
        public IActionResult PayOrder(PaymentViewModel model)
        {
            Console.Write("{0}", model.GivenPay);
            if (ModelState.IsValid)
            {
                Console.WriteLine("{0}", model.GivenPay);
            }
            if (model.Order != null)
            {
                bool payed = model.Order.Pay(model.PaymentMethod, model.GivenPay);
                if (payed)
                {
                    return RedirectToAction(nameof(MyOrders));
                }
            }
            return View(model);
        }
    }
}