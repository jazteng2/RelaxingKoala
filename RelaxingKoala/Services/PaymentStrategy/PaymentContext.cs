using RelaxingKoala.Models;

namespace RelaxingKoala.Services.PaymentStrategy
{
    public class PaymentContext
    {
        private IPaymentMethod _paymentMethod;
        public void SetPaymentStrategy(IPaymentMethod paymentMethod)
        {
            _paymentMethod = paymentMethod;
        }
        public Invoice ProcessPayment()
        {
            return _paymentMethod.ProcessPayment();
        }
    }
}
