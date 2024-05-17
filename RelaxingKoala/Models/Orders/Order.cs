using RelaxingKoala.Models.Users;
using RelaxingKoala.Services.PaymentStrategy;

namespace RelaxingKoala.Models.Orders
{
    public abstract class Order : IOrder
    {
        private Guid _id;
        private Customer _customer;
        private List<MenuItem> _menuItems = new List<MenuItem>();
        private int _cost;
        private OrderState _state= OrderState.onhold;
        private List<Table> _tables = new List<Table>();

        public Order(Guid id, Customer customer)
        {
            _id = id;
            _customer = customer;
        }

        // Properties
        public Guid Id { get { return _id; } }
        public Customer Customer { get { return _customer; } }
        public List<MenuItem> MenuItems { get { return _menuItems; } }
        public int Cost { get { return _cost; } }
        public OrderState State { get { return _state; } }
        public List<Table> Tables { get { return _tables; } }

        // Methods
        public void AddItem(MenuItem item)
        {
            _menuItems.Add(item);
            _cost += item.Cost;
        }
        public void RemoveItem(MenuItem item)
        {
            _menuItems.Remove(item);
            _cost -= item.Cost;
        }
        public void AddTable(Table table)
        {
            _tables.Add(table);
        }
        public void RemoveTable(Table table)
        {
            _tables.Remove(table);
        }
        public abstract Invoice Pay(IPaymentMethod method);
    }
}
