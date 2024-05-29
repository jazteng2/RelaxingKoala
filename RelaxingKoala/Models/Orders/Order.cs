using RelaxingKoala.Models.Users;
using RelaxingKoala.Services.PaymentStrategy;

namespace RelaxingKoala.Models.Orders
{
    public abstract class Order : IOrder
    {
        public Guid Id { get; set; }
        public int Cost { get; set; }
        public Guid UserId { get; set; }
        public OrderState State { get; set; }
        public OrderType Type { get; set; }
        public List<Table> Tables { get; set; } = new List<Table>();
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        
        public abstract Invoice Pay(IPaymentMethod method);
        public void RecalculateCost()
        {
            Cost = 0;
            foreach (var item in MenuItems)
            {
                Cost += item.Cost;
            }
        }
    }
}
