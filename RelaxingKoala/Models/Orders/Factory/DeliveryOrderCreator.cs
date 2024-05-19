using RelaxingKoala.Models.Users;

namespace RelaxingKoala.Models.Orders.Factory
{
    public class DeliveryOrderCreator : OrderCreator
    {
        public DeliveryOrderCreator() { }
        public override IOrder CreateOrder()
        {
            return new DeliveryOrder();
        }
    }
}
