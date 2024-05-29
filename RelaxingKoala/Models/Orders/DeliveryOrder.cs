using RelaxingKoala.Models.Users;
using RelaxingKoala.Services.PaymentStrategy;
using System.Runtime.CompilerServices;
namespace RelaxingKoala.Models.Orders
{
    public class DeliveryOrder : Order
    {
        private readonly int deliveryFee = 5;
        private readonly int serviceFee = 5;
        private readonly int smallOrderFee = 5;
        private readonly int taxFee = 5;
        public override Invoice Pay(IPaymentMethod method)
        {
            PaymentContext context = new PaymentContext();
            context.SetPaymentStrategy(method);
            Invoice invoice = context.ProcessPayment();
            State = OrderState.Payed;
            return invoice;
        }
    }
}
