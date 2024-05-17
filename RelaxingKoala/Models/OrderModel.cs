using System;
using System.Collections.Generic;

namespace RelaxingKoala.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string OrderType { get; set; } // DineIn, TakeAway, Delivery
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public List<string> MenuItems { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
