using RelaxingKoala.Models.Users;

namespace RelaxingKoala.Models.Orders.Factory
{
    public abstract class OrderCreator
    {
        public IOrder GetOrder()
        {
            return CreateOrder();
        }

        public abstract IOrder CreateOrder();
    }
}
