using MySqlConnector;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.Users;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace RelaxingKoala.Data
{
    public class OrderRepository
    {
        private readonly MySqlDataSource _dataSource;
        private readonly TableRepository tableRepo;
        private readonly MenuItemRepository menuItemRepo;
        private readonly UserRepository userRepo;
        public OrderRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
            tableRepo = new TableRepository(dataSource);
            menuItemRepo = new MenuItemRepository(dataSource);
            userRepo = new UserRepository(dataSource);

        }

        public IOrder GetById(Guid id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM customer_order WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id);
            var reader = cmd.ExecuteReader();

            if (!reader.HasRows) return new DineInOrder();
            reader.Read();
            var order = GetOrderObject(reader);

            // Get associated tables and menu items
            var tables = GetTablesById(order.Id);
            var menuItems = GetMenuItemsById(order.Id);
            order.Tables = tables;
            order.MenuItems = menuItems;

            return order;
        }

        public List<IOrder> GetAll()
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM customer_order";
            var reader = cmd.ExecuteReader();

            if (!reader.HasRows) return new List<IOrder>();
            List<IOrder> orders = new List<IOrder>();
            while (reader.Read())
            {
                orders.Add(GetOrderObject(reader));
            }

            // Get associated tables and menu items
            foreach (IOrder o in orders)
            {
                var tables = GetTablesById(o.Id);
                var menuItems = GetMenuItemsById(o.Id);
                o.Tables = tables;
                o.MenuItems = menuItems;
            }

            return orders;
        }

        public List<Table> GetTablesById(Guid id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT t.id, t.tablenumber, t.availability
                FROM dineintable t
                JOIN table_order torder ON t.id = torder.tableId
                WHERE tOrder.customer_orderId = @orderId;
            ";

            cmd.Parameters.AddWithValue("orderId", id);

            List<Table> tables = new List<Table>();
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows) return tables;


            while (reader.Read())
            {
                tables.Add(new Table()
                {
                    Id = reader.GetInt32("id"),
                    Number = reader.GetInt32("tableNumber"),
                    Availability = reader.GetBoolean("availability")
                });
            }

            return tables;
        }
        public List<IOrder> GetAllByState(OrderState state)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM customer_order WHERE orderstateid = @state";
            cmd.Parameters.AddWithValue("state", (int)state + 1);
            List<IOrder> orders = new List<IOrder>();

            var reader = cmd.ExecuteReader();
            if (!reader.HasRows) return orders;
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
        public List<IOrder> GetAllByUserId(Guid id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM customer_order WHERE userid = @userid";
            cmd.Parameters.AddWithValue("userid", id);
            List<IOrder> orders = new List<IOrder>();

            var reader = cmd.ExecuteReader();
            if (!reader.HasRows) return orders;
            while (reader.Read())
            {
                var order = GetOrderObject(reader);
                
                orders.Add(order);
            }

            conn.Close();
            foreach (var order in orders)
            {
                var tables = GetTablesById(order.Id);
                var menuItems = GetMenuItemsById(order.Id);
                order.Tables = tables;
                order.MenuItems = menuItems;
            }

            return orders;
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
            cmd.Parameters.AddWithValue("id", order.Id);
            cmd.Parameters.AddWithValue("cost", order.Cost);
            cmd.Parameters.AddWithValue("userId", order.UserId);
            cmd.Parameters.AddWithValue("orderStateId", (int)order.State + 1);
            cmd.Parameters.AddWithValue("orderTypeId", (int)order.Type + 1);
            cmd.ExecuteNonQuery();

            // Add to table order
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
                    cmd.Parameters.AddWithValue("customer_orderId", order.Id);
                    cmd.ExecuteNonQuery();
                }
            }

            // Add to menuItem order
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
                    cmd.Parameters.AddWithValue("customer_orderId", order.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            cmd.Transaction.Commit();
            conn.Close();
        }

        public bool Update(IOrder order)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            int affectedRows = 0;

            // Update order record
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
            cmd.Parameters.AddWithValue("id", order.Id);
            affectedRows = cmd.ExecuteNonQuery();
            if (affectedRows == 0) return false;

            // Clear existing table associations and menu items
            cmd.Parameters.Clear();
            cmd.CommandText = @"
                DELETE FROM table_order WHERE customer_orderId = @id;
                DELETE FROM menuitem_order WHERE customer_orderId = @id;
            ";
            cmd.Parameters.AddWithValue("id", order.Id);
            affectedRows = 0;
            affectedRows = cmd.ExecuteNonQuery();
            if (affectedRows == 0) return false;

            // Insert new associations
            cmd.Parameters.Clear();
            cmd.CommandText = @"
                INSERT INTO table_order (tableId, customer_orderId)
                VALUES (@tableId, @customer_orderId);
            ";
            foreach (var table in order.Tables)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("tableId", table.Id);
                cmd.Parameters.AddWithValue("customer_orderId", order.Id);
                affectedRows = 0;
                affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows == 0) return false;
            }

            // Insert new menu items
            cmd.Parameters.Clear();
            cmd.CommandText = @"
                INSERT INTO menuitem_order (menuItemId, customer_orderId)
                VALUES (@menuItemId, @customer_orderId);
            ";
            foreach (var item in order.MenuItems)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("menuItemId", item.Id);
                cmd.Parameters.AddWithValue("customer_orderId", order.Id);
                affectedRows = 0;
                affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows == 0) return false;
            }

            cmd.Transaction.Commit();
            conn.Close();
            return true;
        }

        public void Delete(Guid id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM menuitem_order WHERE customer_orderId = @id;
                DElETE FROM table_order WHERE customer_orderId = @id2;
                DELETE FROM customer_order WHERE id = @id3;
            ";
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("id2", id);
            cmd.Parameters.AddWithValue("id3", id);
            cmd.ExecuteNonQuery();
            conn.Close();
        }


        public List<IOrder> PopulateAssociations(List<IOrder> orders)
        {
            using var conn = _dataSource.OpenConnection();

            // Get associated users
            foreach (var o in orders)
            {
                using var userCmd = conn.CreateCommand();
                userCmd.CommandText = @"SELECT * FROM user WHERE id = @id";
                userCmd.Parameters.AddWithValue("id", o.UserId);
                using var reader = userCmd.ExecuteReader();
                reader.Read();
                var role = reader.GetInt32("userRoleId");
                switch (role)
                {
                    case (int)UserRole.Customer + 1:
                        o.User = new Customer()
                        {
                            Id = reader.GetGuid("id"),
                            FirstName = reader.GetString("firstName"),
                            LastName = reader.GetString("lastName"),
                            Email = reader.GetString("email"),
                            Password = reader.GetString("password"),
                            Role = userRepo.GetRole(reader.GetInt32("userRoleId"))
                        };
                        break;

                    case (int)UserRole.Staff + 1:
                        o.User = new Staff()
                        {
                            Id = reader.GetGuid("id"),
                            FirstName = reader.GetString("firstName"),
                            LastName = reader.GetString("lastName"),
                            Email = reader.GetString("email"),
                            Password = reader.GetString("password"),
                            Role = userRepo.GetRole(reader.GetInt32("userRoleId"))
                        };
                        break;
                    default:
                        o.User = new Customer();
                        break;
                }
            }

            // Get associated tables
            foreach (var o in orders)
            {
                using var tableCmd = conn.CreateCommand();
                tableCmd.CommandText = @"
                    SELECT t.id, t.tablenumber, t.availability
                    FROM dineintable t
                    JOIN table_order torder ON t.id = torder.tableId
                    WHERE tOrder.customer_orderId = @orderId;
                ";

                tableCmd.Parameters.AddWithValue("orderId", o.Id);

                List<Table> tables = new List<Table>();
                using var reader = tableCmd.ExecuteReader();

                while (reader.Read())
                {
                    tables.Add(new Table()
                    {
                        Id = reader.GetInt32("id"),
                        Number = reader.GetInt32("tableNumber"),
                        Availability = reader.GetBoolean("availability")
                    });
                }
                o.Tables = tables;
            }

            // Get associated menuitems
            foreach (var o in orders)
            {
                using var menuItemCmd = conn.CreateCommand();
                menuItemCmd.CommandText = @"
                    SELECT m.id, m.title, m.cost, m.availability
                    FROM menuitem m
                    JOIN menuitem_order morder ON m.id = morder.menuItemId
                    WHERE morder.customer_orderId = @orderId;
                ";

                menuItemCmd.Parameters.AddWithValue("orderId", o.Id);

                List<MenuItem> menuItems = new List<MenuItem>();
                using var reader = menuItemCmd.ExecuteReader();

                while (reader.Read())
                {
                    menuItems.Add(new MenuItem()
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("title"),
                        Cost = reader.GetInt32("cost"),
                        Availability = reader.GetBoolean("availability")
                    });
                }
                o.MenuItems = menuItems;
            }

            conn.Close();
            return orders;
        }

        public OrderState GetOrderState(int id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM OrderState WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id);
            var reader = cmd.ExecuteReader();
            reader.Read();
            return (OrderState)Enum.Parse(typeof(OrderState), reader.GetString(1));
        }

        private OrderType GetOrderType(int id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM OrderType WHERE id = @id";
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

        private List<MenuItem> GetMenuItemsById(Guid id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT m.id, m.title, m.cost, m.availability
                FROM menuitem m
                JOIN menuitem_order morder ON m.id = morder.menuItemId
                WHERE morder.customer_orderId = @orderId;
            ";

            cmd.Parameters.AddWithValue("orderId", id);

            List<MenuItem> menuItems = new List<MenuItem>();
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows) return menuItems;

            while (reader.Read())
            {
                menuItems.Add(new MenuItem()
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("title"),
                    Cost = reader.GetInt32("cost"),
                    Availability = reader.GetBoolean("availability")
                });
            }

            return menuItems;
        }
    }
}
