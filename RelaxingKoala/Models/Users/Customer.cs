using RelaxingKoala.Models.Orders;
using System.Numerics;
namespace RelaxingKoala.Models.Users
{
    public class Customer : User
    {
        public List<IOrder> Orders { get; set; } = new List<IOrder>();
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
