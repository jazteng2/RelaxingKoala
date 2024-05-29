using RelaxingKoala.Models.Orders;

namespace RelaxingKoala.Models.Users
{
    public class Staff : User
    {
        public List<IOrder> Orders { get; set; } = new List<IOrder>();
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
