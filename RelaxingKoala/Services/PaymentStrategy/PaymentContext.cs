using RelaxingKoala.Models;
using RelaxingKoala.Models.Orders;

namespace RelaxingKoala.Services.PaymentStrategy
{
    public class PaymentContext
    {
        private IPaymentMethod _paymentMethod;
        public void SetPaymentStrategy(IPaymentMethod paymentMethod)
        {
            _paymentMethod = paymentMethod;
        }
        public bool ProcessPayment(IOrder order, int givenPay)
        {
            return _paymentMethod.ProcessPayment(order, givenPay);
        }
    }
}
