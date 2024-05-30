using RelaxingKoala.Models.Orders;

namespace RelaxingKoala.Models.ViewModels
{
    public class KitchenViewModel
    {
        public List<MenuItem> menuItems { get; set; } = new List<MenuItem>();
        public List<IOrder> orders { get; set; } = new List<IOrder>();
    }
}
