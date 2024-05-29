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
    }
}
