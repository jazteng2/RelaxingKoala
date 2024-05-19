using RelaxingKoala.Models.Users;
using RelaxingKoala.Services.PaymentStrategy;
namespace RelaxingKoala.Models.Orders
{
    public class DeliveryOrder : Order
    {
        public DeliveryOrder() { }
        public override Invoice Pay(IPaymentMethod method)
        {
            PaymentContext context = new PaymentContext();
            context.SetPaymentStrategy(method);
            Invoice invoice = context.ProcessPayment();

            return invoice;
        }
    }
}
