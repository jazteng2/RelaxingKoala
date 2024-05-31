using RelaxingKoala.Models;
using RelaxingKoala.Models.Orders;

namespace RelaxingKoala.Services.PaymentStrategy
{
    public interface IPaymentMethod
    {
        public bool ProcessPayment(IOrder order, int givenPay);
    }
}
