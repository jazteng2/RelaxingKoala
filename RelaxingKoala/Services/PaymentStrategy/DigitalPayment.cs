using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models;

namespace RelaxingKoala.Services.PaymentStrategy
{
    public class DigitalPayment : IPaymentMethod
    {
        private readonly InvoiceRepository invoiceRepo;
        public DigitalPayment(MySqlDataSource dataSource)
        {
            invoiceRepo = new InvoiceRepository(dataSource);
        }
        public bool ProcessPayment(IOrder order, int givenPay)
        {
            if (order.Cost > givenPay) return false;
            Invoice i = new Invoice()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateOnly.FromDateTime(DateTime.Today),
                TotalPay = order.Cost,
                GivenPay = order.Cost,
                ExcessPay = 0,
                PaymentMethod = PaymentMethod.Card,
                OrderId = order.Id,
                UserId = order.UserId,
            };
            invoiceRepo.Insert(i);
            return true;
        }
    }
}
