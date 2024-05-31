using RelaxingKoala.Models;
using MySqlConnector;

namespace RelaxingKoala.Data
{
    public class MenuItemRepository
    {
        private readonly MySqlDataSource _dataSource;
        public MenuItemRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public List<MenuItem> GetAll()
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM menuitem";
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                var list = new List<MenuItem>();
                while (reader.Read())
                {
                    list.Add(new MenuItem()
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("title"),
                        Cost = reader.GetInt32("cost"),
                        Availability = reader.GetBoolean("availability")
                    });
                }
                return list;
            }
            return new List<MenuItem>();
        }

        public MenuItem GetById(int id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM menuitem WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id);
            var reader = cmd.ExecuteReader();
            if (!reader.Read()) return new MenuItem();
            var menuitem = new MenuItem()
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("title"),
                Cost = reader.GetInt32("cost"),
                Availability = reader.GetBoolean("availability")
            };
            return menuitem;
        }

        public bool Update(MenuItem item)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE menuitem SET cost = @cost, availability = @availability WHERE id = @id";
            cmd.Parameters.AddWithValue("cost", item.Cost);
            cmd.Parameters.AddWithValue("availability", item.Availability);
            cmd.Parameters.AddWithValue("id", item.Id);
            int affectedRows = cmd.ExecuteNonQuery();
            conn.Close();

            if (affectedRows > 0) return true;
            return false;
        }

        public List<MenuItem> GetMenuItemsByOrderId(Guid id)
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
