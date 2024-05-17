using RelaxingKoala.Models.Orders;

namespace RelaxingKoala.Data
{
    public interface IOrderRepository
    {
        public void CreateOrder(IOrder order);
        public IOrder ReadOrder(Guid id);
        public void UpdateOrder(IOrder order);
        public void DeleteOrder(IOrder order);

    }
}
