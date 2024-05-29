using MySqlConnector;
using RelaxingKoala.Models.Users;
using System.Data;
using System.Xml.Linq;

namespace RelaxingKoala.Data
{
    public class CustomerRepository
    {
        private readonly MySqlDataSource _dataSource;
        private readonly UserRole _role;
        public CustomerRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
            _role = (int) UserRole.Customer;
        }
        public List<Customer> GetAll()
        {
            List<Customer> customers = new List<Customer>();
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"SELECT * FROM user WHERE userroleid = 1";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                customers.Add(new Customer()
                {
                    Id = reader.GetGuid("id"),
                    FirstName = reader.GetString("firstName"),
                    LastName = reader.GetString("lastName"),
                    Email = reader.GetString("email"),
                    Password = reader.GetString("password")
                });
            }
            conn.Close();
            return customers;
        }

        public Customer GetById(Guid id)
        {
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"SELECT * FROM user WHERE id = @id AND userroleid = @role";
            command.Parameters.AddWithValue("id", id);
            command.Parameters.AddWithValue("role", _role);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Customer()
                {
                    Id = reader.GetGuid("id"),
                    FirstName = reader.GetString("firstName"),
                    LastName = reader.GetString("lastName"),
                    Email = reader.GetString("email"),
                    Password = reader.GetString("password")
                };
            }
            return new Customer();
        }

        public Customer GetByFirstName(string firstName)
        {
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"SELECT * FROM user WHERE firstName = @firstName AND userroleid = @role";
            command.Parameters.AddWithValue("firstName", firstName);
            command.Parameters.AddWithValue("role", _role);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Customer()
                {
                    Id = reader.GetGuid("id"),
                    FirstName = reader.GetString("firstName"),
                    LastName = reader.GetString("lastName"),
                    Email = reader.GetString("email"),
                    Password = reader.GetString("password")
                };
            }
            return new Customer();
        }

        public Customer GetByEmail(string email)
        {
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"SELECT * FROM user WHERE email = @email AND userroleid = @role";
            command.Parameters.AddWithValue("email", email);
            command.Parameters.AddWithValue("role", _role);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Customer()
                {
                    Id = reader.GetGuid("id"),
                    FirstName = reader.GetString("firstName"),
                    LastName = reader.GetString("lastName"),
                    Email = reader.GetString("email"),
                    Password = reader.GetString("password")
                };
            }
            return new Customer();
        }
    }
}
