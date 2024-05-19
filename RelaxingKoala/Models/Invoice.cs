using RelaxingKoala.Models.Users;
using RelaxingKoala.Models.Orders;
using System.Reflection.Metadata.Ecma335;
namespace RelaxingKoala.Models
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public int AmountPayed { get; set; }
        public int Change { get; set; }
        public Guid OrderId { get; set; }
        public required Order Order { get; set; }

        // Methods
        public int CalculateChange ()
        {
            return Order.Cost - AmountPayed;
        }
    }
}
