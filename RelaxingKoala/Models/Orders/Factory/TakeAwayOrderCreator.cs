using RelaxingKoala.Models.Users;

namespace RelaxingKoala.Models.Orders.Factory
{
    public class TakeAwayOrderCreator : OrderCreator
    {
        public TakeAwayOrderCreator() { }
        public override IOrder CreateOrder(Guid id, Customer customer)
        {
            return new TakeAwayOrder(id, customer);
        }
    }
}
