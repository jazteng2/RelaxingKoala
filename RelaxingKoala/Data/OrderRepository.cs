using MySqlConnector;
using RelaxingKoala.Models.Orders;

namespace RelaxingKoala.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MySqlDataSource _dataSource;
        public OrderRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public void CreateOrder(IOrder order)
        {
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"INSERT INTO order";
            command.Parameters.AddWithValue("order", order);    
            command.ExecuteNonQuery();
        }
        public IOrder ReadOrder(Guid id)
        {
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"SELECT * FROM order WHERE orderId = @id";
            command.Parameters.AddWithValue("@id", id);
            var result = command.ExecuteReader().Read();
            return null;
        }
        public void UpdateOrder(IOrder order)
        {
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"INSERT INTO ";
            command.ExecuteNonQuery();
        }

        public void DeleteOrder(IOrder order)
        {
            using var connection = _dataSource.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM `BlogPost` WHERE `Id` = @id;";
            command.Parameters.AddWithValue("@id", order.Id);
            command.ExecuteNonQuery();
        }

        public string Test()
        {
            return "testing the test";
        }
    }
}
