using RelaxingKoala.Services.PaymentStrategy;
namespace RelaxingKoala.Models.Orders
{
    public class DineInOrder : Order
    {
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
