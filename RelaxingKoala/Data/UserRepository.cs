using MySqlConnector;
using RelaxingKoala.Models.Users;

namespace RelaxingKoala.Data
{
    public class UserRepository
    {
        MySqlDataSource _dataSource;
        public UserRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public User GetByEmail(string email)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM user WHERE email = @Email";
            cmd.Parameters.AddWithValue("@Email", email);
            var reader = cmd.ExecuteReader();
            if (!reader.Read()) return new Customer();
            var role = reader.GetInt32("userRoleId");
            switch (role)
            {
                case (int)UserRole.Customer + 1:
                    return new Customer()
                    {
                        Id = reader.GetGuid("id"),
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password")
                    };
                case (int)UserRole.Staff + 1:
                    return new Staff()
                    {
                        Id = reader.GetGuid("id"),
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password")
                    };
                default:
                    return new Customer();
            }
        }

        public void Insert(User user)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO user (id, firstName, lastName, email, password, userRoleId) VALUES (@Id, @FirstName, @LastName, @Email, @Password, @UserRoleId)";
            cmd.Parameters.AddWithValue("@Id", user.Id);
            cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
            cmd.Parameters.AddWithValue("@LastName", user.LastName);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@UserRoleId", user.UserRoleId);
            cmd.ExecuteNonQuery();
        }
    }
}
