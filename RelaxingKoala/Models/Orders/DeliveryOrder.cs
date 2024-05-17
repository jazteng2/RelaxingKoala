using RelaxingKoala.Models.Users;
using RelaxingKoala.Services.PaymentStrategy;
namespace RelaxingKoala.Models.Orders
{
    public class DeliveryOrder : Order
    {
        private int deliveryCost = 2;
        private int serviceCost = 2;
        public DeliveryOrder(Guid id, Customer customer) : base(id, customer)
        {

        }

        public override Invoice Pay(IPaymentMethod method)
        {
            PaymentContext context = new PaymentContext();
            context.SetPaymentStrategy(method);
            Invoice invoice = context.ProcessPayment();

            return invoice;
        }
    }
}
