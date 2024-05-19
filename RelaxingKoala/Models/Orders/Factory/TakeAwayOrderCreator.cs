using RelaxingKoala.Models.Users;

namespace RelaxingKoala.Models.Orders.Factory
{
    public class TakeAwayOrderCreator : OrderCreator
    {
        public TakeAwayOrderCreator() { }
        public override IOrder CreateOrder()
        {
            return new TakeAwayOrder();
        }
    }
}
