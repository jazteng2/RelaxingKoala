using RelaxingKoala.Models.Users;
using RelaxingKoala.Models.Orders;
using System.Reflection.Metadata.Ecma335;
namespace RelaxingKoala.Models
{
    public class Invoice
    {
        private int _id;
        private Order _order;
        private int _amountPayed;
        private int _change;
        public Invoice(int id, Order order, int amountPayed)
        {
            _id = id;
            _order = order;
            _amountPayed = amountPayed;
            _change = CalculateChange();
        }

        // properties
        public int Id {  get { return _id; } }
        public Order Order { get { return _order; } }
        public int AmountPayed { get { return _amountPayed; } }
        public int Change { get { return _change; } }

        // Methods
        private int CalculateChange()
        {
            return _order.Cost - _amountPayed;
        }

    }
}
