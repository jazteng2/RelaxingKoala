using System;
using System.Collections.Generic;

namespace RelaxingKoala.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string OrderType { get; set; } // DineIn, TakeAway, Delivery
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNumber { get; set; }
        public List<string> MenuItems { get; set; } = new List<string>();
        public decimal TotalCost { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string CustomerName => $"{FirstName} {LastName}";
        public string CustomerEmail { get; set; }
    }
}
