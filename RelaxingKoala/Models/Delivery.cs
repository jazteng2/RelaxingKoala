using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.Users;

namespace RelaxingKoala.Models
{
    public class Delivery
    {
        public Guid Id { get; set; }

        // Relationships
        public Guid DriverId { get; set; }
        public required Driver Driver { get; set; }
        public List<DeliveryOrder> Orders { get; set; } = new List<DeliveryOrder>();
    }
}
