using RelaxingKoala.Services.PaymentStrategy;
namespace RelaxingKoala.Models.Orders
{
    public class TakeAwayOrder : Order
    {
        private readonly int serviceFee = 5;
        private readonly int smallOrderFee = 5;
        private readonly int taxFee = 5;
        public override bool Pay(IPaymentMethod method, int givenPay)
        {
            PaymentContext context = new PaymentContext();
            context.SetPaymentStrategy(method);
            int totalCost = serviceFee + smallOrderFee + taxFee;
            Cost = totalCost;
            bool payed = context.ProcessPayment(this, givenPay);
            if (payed) return false;
            State = OrderState.Payed;
            return true;
        }
    }
}