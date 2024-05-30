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
                        Cost = reader.GetInt32("cost"),
                        Name = reader.GetString("title"),
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
            cmd.CommandText = @"SELECT * FROM menuitem";
            var reader = cmd.ExecuteReader();
            if (!reader.Read()) return new MenuItem();
            var menuitem = new MenuItem()
            {
                Id = reader.GetInt32("id"),
                Cost = reader.GetInt32("cost"),
                Name = reader.GetString("title"),
            };
            return menuitem;
        }

        public void Update(MenuItem item)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE menuitem set cost = @cost, availability = @availability WHERE id = @id";
            cmd.Parameters.AddWithValue("cost", item.Cost);
            cmd.Parameters.AddWithValue("availability", item.Availability);
            cmd.Parameters.AddWithValue("id", item.Id);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
