using MySqlConnector;
using RelaxingKoala.Models.Users;

namespace RelaxingKoala.Data
{
    public class UserRepository
    {
        private readonly MySqlDataSource _dataSource;
        public UserRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public User GetByEmail(string email)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM user WHERE email = @email";
            cmd.Parameters.AddWithValue("email", email);
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var name = reader.GetString("firstName");
                var role = reader.GetInt32("userRoleId");
                if (role == (int) UserRole.Customer)
                {
                    return new Customer()
                    {
                        Id = reader.GetGuid("Id"),
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password")
                    };
                } else if (role == (int) UserRole.Staff)
                {
                    return new Staff()
                    {
                        Id = reader.GetGuid("Id"),
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password")
                    };
                }
            }
            return new Customer();
        }
    }
}
