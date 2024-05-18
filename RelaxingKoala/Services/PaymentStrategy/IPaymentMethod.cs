using RelaxingKoala.Models;
using RelaxingKoala.Models.Orders;

namespace RelaxingKoala.Services.PaymentStrategy
{
    public interface IPaymentMethod
    {
        public Invoice ProcessPayment();
    }
}
