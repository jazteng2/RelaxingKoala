using Microsoft.AspNetCore.Mvc;
using RelaxingKoala.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace RelaxingKoala.Controllers
{
    public class OrderController : Controller
    {
        // Display the customer details form
        public IActionResult Create()
        {
            return View(new OrderModel());
        }

        // Handle form submission for customer details
        [HttpPost]
        public IActionResult CreateStep1(OrderModel order)
        {
            if (ModelState.IsValid)
            {
                TempData["Order"] = JsonSerializer.Serialize(order);
                return RedirectToAction("SelectOrderType");
            }
            return View("SelectOrderType", order);
        }

        // Display the order type selection form
        public IActionResult SelectOrderType()
        {
            if (TempData.ContainsKey("Order"))
            {
                var order = JsonSerializer.Deserialize<OrderModel>((string)TempData["Order"]);
                TempData.Keep("Order");
                return View(order);
            }
            return RedirectToAction("Create");
        }

        // Handle form submission for order type
        [HttpPost]
        public IActionResult SelectOrderTypeStep2(OrderModel order)
        {
            if (ModelState.IsValid)
            {
                TempData["Order"] = JsonSerializer.Serialize(order);
                return RedirectToAction("SelectMenuItems");
            }
            return View("SelectMenuItem", order);
        }

        // Display the menu items selection form
        public IActionResult SelectMenuItems()
        {
            if (TempData.ContainsKey("Order"))
            {
                var order = JsonSerializer.Deserialize<OrderModel>((string)TempData["Order"]);
                TempData.Keep("Order");
                ViewBag.MenuItems = new List<string> { "Pizza - $10", "Salad - $5", "Burger - $8" };
                return View(order);
            }
            return RedirectToAction("Create");
        }

        // Handle form submission for menu items and calculate total cost
        [HttpPost]
        public IActionResult SelectMenuItemsStep3(OrderModel order, List<string> selectedItems)
        {
            if (ModelState.IsValid)
            {
                order.MenuItems = selectedItems;
                order.TotalCost = selectedItems.Count * 10; // Example cost calculation
                TempData["Order"] = JsonSerializer.Serialize(order);
                return RedirectToAction("OrderSummary");
            }
            ViewBag.MenuItems = new List<string> { "Pizza - $10", "Salad - $5", "Burger - $8" };
            return View("OrderSummary", order);
        }

        // Display the order summary
        public IActionResult OrderSummary()
        {
            if (TempData.ContainsKey("Order"))
            {
                var order = JsonSerializer.Deserialize<OrderModel>((string)TempData["Order"]);
                return View(order);
            }
            return RedirectToAction("Create");
        }

        // Handle form submission for final order
        [HttpPost]
        public IActionResult SubmitOrder()
        {
            if (TempData.ContainsKey("Order"))
            {
                var order = JsonSerializer.Deserialize<OrderModel>((string)TempData["Order"]);
                if (ModelState.IsValid)
                {
                    // Save the order to the database (implementation not shown)
                    return RedirectToAction("Index");
                }
                return View("OrderSummary", order);
            }
            return RedirectToAction("Create");
        }
    }
}
