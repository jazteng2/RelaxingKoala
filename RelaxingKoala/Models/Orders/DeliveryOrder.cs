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
        public override bool Pay(IPaymentMethod method, int givenPay)
        {
            PaymentContext context = new PaymentContext();
            context.SetPaymentStrategy(method);
            int totalCost = deliveryFee + serviceFee + smallOrderFee + taxFee + Cost;
            Cost = totalCost;
            bool payed = context.ProcessPayment(this, givenPay);
            if (!payed) return false;
            State = OrderState.Payed;
            return true;
        }
    }
}
