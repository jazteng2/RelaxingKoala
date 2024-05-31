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
            DateOnly startDate = new DateOnly(2024, 1, 1);
            DateOnly endDate = new DateOnly(2025, 12, 31);

            foreach (var order in orders)
            {
                var invoice = new Invoice()
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = GetRandomDate(startDate, endDate),
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

        [Fact]
        public void TestGetById()
        {
            var invoice = invoiceRepo.GetById(new Guid("08f7809b-ffc7-48e4-af71-fa7203a1c93f"));
            DisplayInvoice(invoice);
        }

        [Fact]
        public void TestGetInvoicesByUserId()
        {
            var invoices = invoiceRepo.GetInvoicesByUserId(new Guid("672ecc3a-c4cb-4569-b8d3-2e62bef2b215"));
            foreach (var invoice in invoices)
            {
                DisplayInvoice(invoice);
            }
        }

        [Fact]
        public void TestGetByOrderId()
        {
            var invoice = invoiceRepo.GetByOrderId(new Guid("6950be07-83f1-478c-9e2c-d491322011c7"));
            DisplayInvoice(invoice);
        }

        [Fact]
        public void TestGetByPeriod()
        {
            DateOnly startDate = new DateOnly(2024, 1, 1);
            DateOnly endDate = new DateOnly(2025, 12, 31);
            var invoices = invoiceRepo.GetByPeriod(startDate, endDate);
            foreach (var invoice in invoices)
            {
                DisplayInvoice(invoice);
            }
        }

        private void DisplayInvoice(Invoice i)
        {
            output.WriteLine(
                "{0} {1} {2} {3} {4} {5} {6} {7}", 
                i.Id, i.CreatedDate, i.TotalPay, i.GivenPay, i.ExcessPay, i.PaymentMethod, i.OrderId, i.UserId);
        }

        private DateOnly GetRandomDate(DateOnly startDate, DateOnly endDate)
        {
            // Calculate the total number of days between the start and end dates
            int totalDays = (endDate.ToDateTime(TimeOnly.MinValue) - startDate.ToDateTime(TimeOnly.MinValue)).Days;

            // Create an instance of Random class
            Random random = new Random();

            // Generate a random number of days within the range
            int randomDays = random.Next(totalDays + 1); // +1 to include the end date

            // Add the random number of days to the start date
            DateOnly randomDate = startDate.AddDays(randomDays);

            return randomDate;
        }
    }
}
