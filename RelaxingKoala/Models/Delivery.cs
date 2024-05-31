using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.Users;

namespace RelaxingKoala.Models
{
    public class Delivery
    {
        public Guid Id { get; set; }

        // Relationships
        public Guid DriverId { get; set; }
        public Guid OrderId { get; set; }
        public Driver Driver { get; set; } = new Driver();
        public IOrder? Order { get; set; }
    }
}
