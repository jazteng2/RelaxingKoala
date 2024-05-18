using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.Users;

namespace TestRelaxingKoala
{
    public class TestOrder
    {
        private readonly OrderRepository repository;
        public TestOrder()
        {
            repository = new OrderRepository(new MySqlDataSource(""));
        }

        // CRUD Database
        [Fact]
        public void TestFetchOrderDatabase()
        {
            Assert.Equal("testing the test", repository.Test());
        }
        [Fact]
        public void TestCreateOrderDatabase()
        {
            IOrder order = new DineInOrder(new Guid(), new Customer(new Guid(), "jared", "jared@gmail.com"))
        }
    }
}