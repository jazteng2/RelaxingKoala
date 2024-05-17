using RelaxingKoala.Models.Orders;
using System.Numerics;
namespace RelaxingKoala.Models.Users
{
    public class Customer : User
    {
        private List<IOrder> _orders = new List<IOrder>();
        private List<Invoice> _invoices = new List<Invoice>();
        public Customer(int id, string name, string email) : base(id, name, email) { }
        public List<IOrder> Orders { get { return _orders; } }
        public List<Invoice> Invoices { get {  return _invoices; } }
    }
}
