using RelaxingKoala.Data;
using RelaxingKoala.Models.Users;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.Orders.Factory;

namespace RelaxingKoala.Services
{
    public class OrderService
    {
        public IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public IOrder CreateDeliveryOrder(Customer customer)
        {
            OrderCreator _orderCreator = new DeliveryOrderCreator();
            Guid id = Guid.NewGuid();
            IOrder order = _orderCreator.CreateOrder();
            _orderRepository.CreateOrder(order);
            return order;
        }
        public IOrder CreateDineInOrder(Customer customer)
        {
            OrderCreator _orderCreator = new DeliveryOrderCreator();
            Guid id = Guid.NewGuid();
            IOrder order = _orderCreator.CreateOrder();
            _orderRepository.CreateOrder(order);
            return order;
        }
        public IOrder CreateTakeAwayOrder(Customer customer)
        {
            OrderCreator _orderCreator = new DeliveryOrderCreator();
            Guid id = Guid.NewGuid();
            IOrder order = _orderCreator.CreateOrder();
            _orderRepository.CreateOrder(order);
            return order;
        }

        public void UpdateOrder(IOrder order)
        {
            _orderRepository.UpdateOrder(order);
        }
        public void DeleteOrder(IOrder order)
        {
            _orderRepository.DeleteOrder(order);
        }
    }
}
