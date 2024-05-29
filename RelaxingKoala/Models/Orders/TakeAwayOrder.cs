using RelaxingKoala.Services.PaymentStrategy;
namespace RelaxingKoala.Models.Orders
{
    public class TakeAwayOrder : Order
    {
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