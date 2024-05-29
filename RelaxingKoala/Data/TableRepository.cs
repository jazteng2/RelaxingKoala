using MySqlConnector;
using RelaxingKoala.Models;

namespace RelaxingKoala.Data
{
    public class TableRepository
    {
        private readonly MySqlDataSource _dataSource;
        public TableRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public List<Table> GetAll()
        {
            List<Table> list = new List<Table>();
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"SELECT * FROM DineInTable";
            var reader =  command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Table
                {
                    Id = reader.GetInt32("id"),
                    Number = reader.GetInt32("tableNumber"),
                    Availability = reader.GetBoolean("availability")
                });
            }

            return list;
        }

        public Table GetById(int id)
        {
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"SELECT * FROM DineInTable WHERE id = @id";
            command.Parameters.AddWithValue("id", id);
            var reader = command.ExecuteReader();
            reader.Read();
            return new Table()
            {
                Id = reader.GetInt32("id"),
                Number = reader.GetInt32("tableNumber"),
                Availability = reader.GetBoolean("availability")
            };
        }

        public int Insert(Table table)
        {
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            // INSERT
            command.CommandText = @"
                INSERT INTO DineInTable (tableNumber, availability)
                VALUES (@number, @availability);
                SELECT LAST_INSERT_ID();
            ";
            command.Parameters.AddWithValue("number", table.Number);
            command.Parameters.AddWithValue("availability", table.Availability);
            var reader = command.ExecuteReader();
            reader.Read();
            return reader.GetInt32("LAST_INSERT_ID()");   
        }

        public void Update(Table table)
        {
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"UPDATE dineintable SET availability = @availability WHERE id = @id";
            command.Parameters.AddWithValue("id", table.Id);
            command.Parameters.AddWithValue("availability", table.Availability);
            command.ExecuteNonQuery();
        }
    }
}
