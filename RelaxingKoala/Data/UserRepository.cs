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
            cmd.CommandText = @"SELECT * FROM user WHERE email = @Email";
            cmd.Parameters.AddWithValue("@Email", email);
            var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null; // Return null if no user is found

            var roleString = reader.GetString("userRoleId");
            if (!int.TryParse(roleString, out int role))
            {
                // Handle the error or default to a specific role
                role = (int)UserRole.Customer; // Default to Customer if conversion fails
            }

            switch (role)
            {
                case (int)UserRole.Customer:
                    return new Customer()
                    {
                        Id = Guid.Parse(reader.GetString("id")),
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password")
                    };
                case (int)UserRole.Staff:
                    return new Staff()
                    {
                        Id = Guid.Parse(reader.GetString("id")),
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password")
                    };
                default:
                    return null; // Return null for unknown roles
            }
        }


        public void Insert(User user)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO user (id, firstName, lastName, email, password, userRoleId) VALUES (@Id, @FirstName, @LastName, @Email, @Password, @UserRoleId)";
            cmd.Parameters.AddWithValue("@Id", user.Id.ToString());
            cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
            cmd.Parameters.AddWithValue("@LastName", user.LastName);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@UserRoleId", (int)user.UserRoleId);
            cmd.ExecuteNonQuery();
        }
    }
}
