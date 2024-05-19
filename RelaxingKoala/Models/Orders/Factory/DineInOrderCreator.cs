using RelaxingKoala.Models.Users;

namespace RelaxingKoala.Models.Orders.Factory
{
    public class DineInOrderCreator : OrderCreator
    {
        public DineInOrderCreator() { }
        public override IOrder CreateOrder()
        {
            return new DineInOrder();
        }
    }
}
