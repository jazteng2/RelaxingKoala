using Microsoft.AspNetCore.Mvc;
using RelaxingKoala.Models;
using System;
using System.Collections.Generic;

namespace RelaxingKoala.Controllers
{
    public class OrderController : Controller
    {
        // Display a list of orders
        public IActionResult Index()
        {
            // Placeholder data for orders
            var orders = new List<OrderModel>
            {
                new OrderModel { Id = 1, OrderType = "DineIn", CustomerName = "Jane Smith", CustomerEmail = "jane@example.com", MenuItems = new List<string> { "Pizza", "Salad" }, TotalCost = 29.99m, OrderDate = DateTime.Now }
            };

            return View(orders);
        }

        // Display the order creation form
        public IActionResult Create()
        {
            return View();
        }

        // Handle form submission for order creation
        [HttpPost]
        public IActionResult Create(OrderModel order)
        {
            if (ModelState.IsValid)
            {
                // Save the order to the database (implementation not shown)
                return RedirectToAction("Index");
            }
            return View(order);
        }
    }
}
