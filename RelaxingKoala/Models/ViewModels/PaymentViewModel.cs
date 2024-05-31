using RelaxingKoala.Models.Orders;
using RelaxingKoala.Services.PaymentStrategy;

namespace RelaxingKoala.Models.ViewModels
{
    public class PaymentViewModel
    {
        public IPaymentMethod PaymentMethod { get; set; }
        public IOrder Order { get; set; }
        public int GivenPay { get; set; }
    }
}
