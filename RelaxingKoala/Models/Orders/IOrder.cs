using RelaxingKoala.Models.Users;
using RelaxingKoala.Services.PaymentStrategy;

namespace RelaxingKoala.Models.Orders
{
    public interface IOrder
    {
        // Properties
        public Guid Id { get; }
        public Customer Customer { get; }
        public List<MenuItem> MenuItems { get; }
        public int Cost { get; }
        public OrderState State { get; }
        public List<Table> Tables { get; }
        // Methods
        public void AddItem(MenuItem item);
        public void RemoveItem(MenuItem item);
        public void AddTable(Table table);
        public void RemoveTable(Table table);
        public abstract Invoice Pay(IPaymentMethod method);
    }
}
