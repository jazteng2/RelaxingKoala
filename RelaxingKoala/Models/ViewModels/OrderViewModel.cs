using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RelaxingKoala.Models.Orders
{
    public class OrderViewModel
    {
        public Guid UserId { get; set; }

        [Required]
        public OrderType OrderTypeId { get; set; }

        [Required]
        public OrderState OrderStateId { get; set; }

        [Required]
        public List<int> MenuItems { get; set; }

        public List<int> Tables { get; set; } = new List<int>();

        public int Cost { get; set; }
    }
}
