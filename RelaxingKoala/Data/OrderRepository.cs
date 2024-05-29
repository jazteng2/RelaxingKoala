using MySqlConnector;
using RelaxingKoala.Models.Orders;

namespace RelaxingKoala.Data
{
    public class OrderRepository
    {
        private readonly MySqlDataSource _dataSource;
        public OrderRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public void Insert(IOrder order)
        {
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"
                INSERT INTO order (id, cost, userId, orderStateId, orderTypeId)
                VALUES (@id, @cost, @userId, @orderStateId, @orderTypeId)            
            ";
            command.Parameters.AddWithValue("id", order.Id);
            command.Parameters.AddWithValue("cost", order.Cost);
            command.Parameters.AddWithValue("userId", order.UserId);
            command.Parameters.AddWithValue("orderStateId", (int)order.State);
            command.Parameters.AddWithValue("orderStateId", (int)order.Type);
            command.ExecuteNonQuery();
            conn.Close();

            // Add to table order
            if (order.Tables.Count > 0)
            {
                using var conn2 = _dataSource.OpenConnection();
                var t = conn2.BeginTransaction();
                using var command2 = conn2.CreateCommand();
                command2.CommandText = @"
                    INSERT INTO table_order (tableId, customer_orderId)
                    VALUES (@tableId, @customer_orderId)
                ";

                foreach (var table in order.Tables)
                {
                    command2.Parameters.Clear();
                    command2.Parameters.AddWithValue("tableId", table.Id);
                    command2.Parameters.AddWithValue("customer_orderId", order.Id);
                    command2.ExecuteNonQuery();
                }

                t.Commit();
                conn2.Close();
            }

            // Add to menuItem order
            if (order.MenuItems.Count > 0)
            {
                using var conn2 = _dataSource.OpenConnection();
                var t = conn2.BeginTransaction();
                using var command2 = conn2.CreateCommand();
                command2.CommandText = @"
                    INSERT INTO menuItem_order (menuItemId, customer_orderId)
                    VALUES (@menuItemId, @customer_orderId)
                ";

                foreach (var menuItem in order.MenuItems)
                {
                    command2.Parameters.Clear();
                    command2.Parameters.AddWithValue("menuItemId", menuItem.Id);
                    command2.Parameters.AddWithValue("customer_orderId", order.Id);
                    command2.ExecuteNonQuery();
                }

                t.Commit();
                conn2.Close();
            }
        }
    }
}
