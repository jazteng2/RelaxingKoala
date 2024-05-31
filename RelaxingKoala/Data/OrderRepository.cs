using MySqlConnector;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.Users;
using System.Collections.Generic;

namespace RelaxingKoala.Data
{
    public class OrderRepository
    {
        private readonly MySqlDataSource _dataSource;

        public OrderRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public IOrder GetById(Guid id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM customer_order WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id.ToString());
            var reader = cmd.ExecuteReader();

            if (!reader.HasRows) return new DineInOrder();
            reader.Read();
            var order = GetOrderObject(reader);

            // Get associated tables and menu items
            var tables = tableRepo.GetTablesByOrderId(order.Id);
            var menuItems = menuItemRepo.GetMenuItemsByOrderId(order.Id);
            order.Tables = tables;
            order.MenuItems = menuItems;

            return order;
        }

        public void Insert(IOrder order)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.Transaction = conn.BeginTransaction();
            cmd.CommandText = @"
                INSERT INTO customer_order (id, cost, userId, orderStateId, orderTypeId)
                VALUES (@id, @cost, @userId, @orderStateId, @orderTypeId)            
            ";
            cmd.Parameters.AddWithValue("id", order.Id.ToString());
            cmd.Parameters.AddWithValue("cost", order.Cost);
            cmd.Parameters.AddWithValue("userId", order.UserId.ToString());
            cmd.Parameters.AddWithValue("orderStateId", (int)order.State + 1);
            cmd.Parameters.AddWithValue("orderTypeId", (int)order.Type + 1);
            cmd.ExecuteNonQuery();

            if (order.Tables.Count > 0)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = @"
                    INSERT INTO table_order (tableId, customer_orderId)
                    VALUES (@tableId, @customer_orderId)
                ";
                foreach (var table in order.Tables)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("tableId", table.Id);
                    cmd.Parameters.AddWithValue("customer_orderId", order.Id.ToString());
                    cmd.ExecuteNonQuery();
                }
            }

            if (order.MenuItems.Count > 0)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = @"
                    INSERT INTO menuItem_order (menuItemId, customer_orderId)
                    VALUES (@menuItemId, @customer_orderId)
                ";
                foreach (var menuItem in order.MenuItems)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("menuItemId", menuItem.Id);
                    cmd.Parameters.AddWithValue("customer_orderId", order.Id.ToString());
                    cmd.ExecuteNonQuery();
                }
            }
            cmd.Transaction.Commit();
            conn.Close();
        }
        public List<IOrder> GetAll()
        {
            var orders = new List<IOrder>();
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM customer_order";
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var order = GetOrderObject(reader);
                var tables = GetTablesById(order.Id);
                var menuItems = GetMenuItemsById(order.Id);
                order.Tables = tables;
                order.MenuItems = menuItems;
                orders.Add(order);
            }
            return orders;
        }
        public List<IOrder> GetAllByState(OrderState state)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM customer_order WHERE orderStateId = @state";
            cmd.Parameters.AddWithValue("state", (int)state + 1);
            var orders = new List<IOrder>();

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var order = GetOrderObject(reader);
                var tables = GetTablesById(order.Id);
                var menuItems = GetMenuItemsById(order.Id);
                order.Tables = tables;
                order.MenuItems = menuItems;
                orders.Add(order);
            }

            return orders;
        }

        public List<IOrder> PopulateAssociations(List<IOrder> orders)
        {
            foreach (var order in orders)
            {
                order.Tables = GetTablesById(order.Id);
                order.MenuItems = GetMenuItemsById(order.Id);
            }
            return orders;
        }





        public bool Update(IOrder order)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            int affectedRows = 0;

            cmd.Transaction = conn.BeginTransaction();
            cmd.CommandText = @"
                UPDATE customer_order SET 
                    cost = @cost, 
                    orderStateId = @state, 
                    orderTypeId = @type 
                WHERE id = @id
            ";
            cmd.Parameters.AddWithValue("cost", order.Cost);
            cmd.Parameters.AddWithValue("state", (int)order.State + 1);
            cmd.Parameters.AddWithValue("type", (int)order.Type + 1);
            cmd.Parameters.AddWithValue("id", order.Id.ToString());
            affectedRows = cmd.ExecuteNonQuery();
            if (affectedRows == 0) return false;

            cmd.Parameters.Clear();
            cmd.CommandText = @"
                DELETE FROM table_order WHERE customer_orderId = @id;
                DELETE FROM menuitem_order WHERE customer_orderId = @id;
            ";
            cmd.Parameters.AddWithValue("id", order.Id.ToString());
            affectedRows = cmd.ExecuteNonQuery();

            if (order.Tables.Count > 0)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = @"
                    INSERT INTO table_order (tableId, customer_orderId)
                    VALUES (@tableId, @customer_orderId)
                ";
                foreach (var table in order.Tables)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("tableId", table.Id);
                    cmd.Parameters.AddWithValue("customer_orderId", order.Id.ToString());
                    cmd.ExecuteNonQuery();
                }
            }

            if (order.MenuItems.Count > 0)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = @"
                    INSERT INTO menuitem_order (menuItemId, customer_orderId)
                    VALUES (@menuItemId, @customer_orderId)
                ";
                foreach (var menuItem in order.MenuItems)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("menuItemId", menuItem.Id);
                    cmd.Parameters.AddWithValue("customer_orderId", order.Id.ToString());
                    cmd.ExecuteNonQuery();
                }
            }
            cmd.Transaction.Commit();
            conn.Close();
            return true;
        }

        public void Delete(Guid id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM customer_order WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id.ToString());
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public List<IOrder> GetByUserId(Guid userId)
        {
            var orders = new List<IOrder>();
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM customer_order WHERE userId = @userId";
            cmd.Parameters.AddWithValue("userId", userId.ToString());
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var order = GetOrderObject(reader);
                var tables = tableRepo.GetTablesByOrderId(order.Id);
                var menuItems = menuItemRepo.GetMenuItemsByOrderId(order.Id);
                order.Tables = tables;
                order.MenuItems = menuItems;
                orders.Add(order);
            }
            return orders;
        }

        public List<Table> GetAvailableTables()
        {
            var tables = new List<Table>();
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, tableNumber, availability FROM dineintable WHERE availability = TRUE";
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tables.Add(new Table
                {
                    Id = reader.GetInt32("id"),
                    Number = reader.GetInt32("tableNumber"),
                    Availability = reader.GetBoolean("availability")
                });
            }
            return tables;
        }


        public List<MenuItem> GetAvailableMenuItems()
        {
            var menuItems = new List<MenuItem>();
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, title, cost, availability FROM menuitem WHERE availability = TRUE";
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                menuItems.Add(new MenuItem
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("title"),
                    Cost = reader.GetInt32("cost"),
                    Availability = reader.GetBoolean("availability")
                });
            }
            return menuItems;
        }

        private IOrder GetOrderObject(MySqlDataReader reader)
        {
            var type = reader.GetInt32("orderTypeId");
            return type switch
            {
                (int)OrderType.DineIn + 1 => new DineInOrder
                {
                    Id = Guid.Parse(reader.GetString("id")),
                    Cost = reader.GetInt32("cost"),
                    UserId = Guid.Parse(reader.GetString("userId")),
                    State = GetOrderState(reader.GetInt32("orderStateId")),
                    Type = GetOrderType(type)
                },
                (int)OrderType.TakeAway + 1 => new TakeAwayOrder
                {
                    Id = Guid.Parse(reader.GetString("id")),
                    Cost = reader.GetInt32("cost"),
                    UserId = Guid.Parse(reader.GetString("userId")),
                    State = GetOrderState(reader.GetInt32("orderStateId")),
                    Type = GetOrderType(type)
                },
                (int)OrderType.Delivery + 1 => new DeliveryOrder
                {
                    Id = Guid.Parse(reader.GetString("id")),
                    Cost = reader.GetInt32("cost"),
                    UserId = Guid.Parse(reader.GetString("userId")),
                    State = GetOrderState(reader.GetInt32("orderStateId")),
                    Type = GetOrderType(type)
                },
                _ => new DineInOrder()
            };
        }

        public OrderState GetOrderState(int id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM orderState WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id);
            var reader = cmd.ExecuteReader();
            reader.Read();
            return (OrderState)Enum.Parse(typeof(OrderState), reader.GetString(1));
        }

        private OrderType GetOrderType(int id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM orderType WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id);
            var reader = cmd.ExecuteReader();
            reader.Read();
            return (OrderType)Enum.Parse(typeof(OrderType), reader.GetString(1));
        }

        public IOrder GetOrderObject(MySqlDataReader reader)
        {
            var type = reader.GetInt32("orderTypeId");
            switch (type)
            {
                case (int)OrderType.DineIn + 1:
                    return new DineInOrder()
                    {
                        Id = reader.GetGuid("id"),
                        Cost = reader.GetInt32("cost"),
                        UserId = reader.GetGuid("userId"),
                        State = GetOrderState(reader.GetInt32("orderStateId")),
                        Type = GetOrderType(type)
                    };
                case (int)OrderType.TakeAway + 1:
                    return new TakeAwayOrder()
                    {
                        Id = reader.GetGuid("id"),
                        Cost = reader.GetInt32("cost"),
                        UserId = reader.GetGuid("userId"),
                        State = GetOrderState(reader.GetInt32("orderStateId")),
                        Type = GetOrderType(type)
                    };
                case (int)OrderType.Delivery + 1:
                    return new DeliveryOrder()
                    {
                        Id = reader.GetGuid("id"),
                        Cost = reader.GetInt32("cost"),
                        UserId = reader.GetGuid("userId"),
                        State = GetOrderState(reader.GetInt32("orderStateId")),
                        Type = GetOrderType(type)
                    };
                default:
                    return new DineInOrder();
            }
        }

        
    }
}
