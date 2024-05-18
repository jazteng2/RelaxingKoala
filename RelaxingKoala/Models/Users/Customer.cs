using RelaxingKoala.Models.Orders;
using System.Numerics;
namespace RelaxingKoala.Models.Users
{
    public class Customer : User
    {
        private List<IOrder> _orders = new List<IOrder>();
        private List<Invoice> _invoices = new List<Invoice>();
        private List<Reservation> _reservtions = new List<Reservation>();
        public Customer(Guid id, string name, string email) : base(id, name, email) { }

        // Properties
        public List<IOrder> Orders { get { return _orders; } }
        public List<Invoice> Invoices { get {  return _invoices; } }
        public List<Reservation> Reservations { get {  return _reservtions; } }

        // Methods
        public void AddOrder(IOrder order)
        {
            _orders.Add(order);
        }
        public void RemoveOrder(IOrder order)
        {
            _orders.Remove(order);
        }
        public IOrder GetOrder(Guid id)
        {
            foreach (var item in _orders)
            {
                if (item.Id.Equals(id))
                {
                    return item;
                }
            }
            return null;
        }

        public void AddInvoice(Invoice invoice)
        {
            _invoices.Add(invoice);
        }
        public void RemoveInvoice(Invoice invoice)
        {
            _invoices.Remove(invoice);
        }
        public Invoice GetInvoice(Guid id)
        {
            foreach (var item in _invoices)
            {
                if (item.Id.Equals(id))
                {
                    return item;
                }
            }
            return null;
        }
        public void AddReservation(Reservation reservation)
        {
            _reservtions.Add(reservation);
        }
        public void RemoveReserervation(Reservation reservation)
        {
            _reservtions.Remove(reservation);
        }
        public Reservation GetReservation(Guid id)
        {
            foreach (var item in _reservtions)
            {
                if (item.Id.Equals(id))
                {
                    return item;
                }
            }
            return null;
        }
    }
}
