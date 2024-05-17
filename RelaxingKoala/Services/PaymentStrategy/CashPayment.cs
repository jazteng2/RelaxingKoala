using RelaxingKoala.Models.Orders;

namespace RelaxingKoala.Services.PaymentStrategy
{
    public class CashPayment : IPaymentMethod
    {
        public bool ProcessOrder()
        {
            return true;
        }
    }
}
