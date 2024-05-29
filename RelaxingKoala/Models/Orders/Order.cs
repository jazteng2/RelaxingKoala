using RelaxingKoala.Models.Users;
using RelaxingKoala.Services.PaymentStrategy;

namespace RelaxingKoala.Models.Orders
{
    public abstract class Order : IOrder
    {
        public Guid Id { get; set; }
        public int Cost { get; set; }
        public OrderState State { get; set; }

        // Relationships
        public Guid CustomerId { get; set; }
        public List<Table> Tables { get; set; } = new List<Table>();
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        // Methods
        public abstract Invoice Pay(IPaymentMethod method);
        public void AddItem(MenuItem item)
        {
            MenuItems.Add(item);
            Cost += item.Cost;
        }
    }
}
