using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Orders;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace TestRelaxingKoala
{
    public class TestInvoice
    {
        private readonly ITestOutputHelper output;
        private readonly OrderRepository orderRepo;
        private readonly CustomerRepository customerRepo;
        private readonly InvoiceRepository invoiceRepo;
        public TestInvoice(ITestOutputHelper output)
        {
            var conn = new MySqlDataSource("Server=127.0.0.1;Port=3306;User ID=root;Password=admin;Database=rkdb");
            this.output = output;
            orderRepo = new OrderRepository(conn);
            customerRepo = new CustomerRepository(conn);
            invoiceRepo = new InvoiceRepository(conn);
        }

        [Fact]
        public void TestInsertInvoices()
        {
            var orders = orderRepo.GetAllByState(OrderState.Complete);
            var rand = new Random();
            foreach (var order in orders)
            {
                var invoice = new Invoice()
                {
                    Id = order.Id,
                    TotalPay = order.Cost,
                    GivenPay = rand.Next(60, 100),
                    ExcessPay = 60 - order.Cost,
                    PaymentMethod = PaymentMethod.Cash,
                    OrderId = order.Id,
                    UserId = order.UserId
                };

                invoiceRepo.Insert(invoice);
            }
        }

        private void DisplayInvoice(Invoice i)
        {
            output.WriteLine("{0} {1} {2} {3} {4} {5} {6}", i.Id, i.TotalPay, i.GivenPay, i.ExcessPay, i.PaymentMethod, i.OrderId, i.UserId);
        }
    }
}
