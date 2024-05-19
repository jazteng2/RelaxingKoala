using RelaxingKoala.Models.Orders;

namespace RelaxingKoala.Data
{
    public interface IOrderRepository
    {
        public void CreateOrder(IOrder order);
        public DeliveryOrder? GetDeliveryOrder(Guid id);
        public void UpdateOrder(IOrder order);
        public void DeleteOrder(IOrder order);

    }
}
