using RelaxingKoala.Models;
using RelaxingKoala.Models.Orders;

namespace RelaxingKoala.Services.PaymentStrategy
{
    public class CashPayment : IPaymentMethod
    {
        public Invoice ProcessPayment()
        {
            return null;
        }
    }
}
