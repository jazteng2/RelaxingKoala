using RelaxingKoala.Models.Users;

namespace RelaxingKoala.Models.Orders.Factory
{
    public abstract class OrderCreator
    {
        public IOrder GetOrder(Guid id, Customer customer)
        {
            return CreateOrder(id, customer);
        }

        public abstract IOrder CreateOrder(Guid id, Customer customer);
    }
}
