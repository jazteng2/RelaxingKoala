using RelaxingKoala.Models.Orders;

namespace RelaxingKoala.Services.PaymentStrategy
{
    public interface IPaymentMethod
    {
        public bool ProcessOrder();
    }
}
