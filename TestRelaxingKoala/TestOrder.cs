using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.Users;
using Xunit.Abstractions;

namespace TestRelaxingKoala
{
    public class TestOrder
    {
        private readonly ITestOutputHelper output;
        private readonly OrderRepository orderRepo;
        private readonly CustomerRepository customerRepo;
        public TestOrder(ITestOutputHelper output)
        {
            var conn = new MySqlDataSource("Server=127.0.0.1;Port=3306;User ID=root;Password=admin;Database=rkdb");
            this.output = output;
            orderRepo = new OrderRepository(conn);
            customerRepo = new CustomerRepository(conn);
        }

        [Fact]
        public void TestInsertOrder()
        {
            Customer c = customerRepo.GetByFirstName("Emily");
            DeliveryOrder order = new DeliveryOrder()
            {
                Id = Guid.NewGuid(),
                Cost = 0,
                UserId = c.Id,
                State = OrderState.Onhold,
                Type = OrderType.Delivery,
            };
            output.WriteLine("{0} {1} {2} {3} {4}", order.Id, order.Cost, order.UserId, order.State, order.Type);
        }
    }
}
