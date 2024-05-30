using RelaxingKoala.Models.Users;
using RelaxingKoala.Models.Orders;
using System.Reflection.Metadata.Ecma335;
using RelaxingKoala.Services.PaymentStrategy;
namespace RelaxingKoala.Models
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public DateOnly CreatedDate { get; set; }
        public int TotalPay { get; set; }
        public int GivenPay { get; set; }
        public int ExcessPay { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
    }
}
