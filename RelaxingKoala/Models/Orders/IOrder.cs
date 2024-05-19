using RelaxingKoala.Models.Users;
using RelaxingKoala.Services.PaymentStrategy;

namespace RelaxingKoala.Models.Orders
{
    public interface IOrder
    {
        public Guid Id { get; set; }
        public int Cost { get; set; }
        public OrderState State { get; set; }
        public List<MenuItem> MenuItems { get; set; }
        public List<Table> Tables { get; set; }
        public Guid CustomerId { get; set; }
        public abstract Invoice Pay(IPaymentMethod method);
    }
}
