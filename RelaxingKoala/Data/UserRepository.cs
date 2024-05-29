using MySqlConnector;
using RelaxingKoala.Models.Users;
using System;

namespace RelaxingKoala.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User GetByEmail(string email)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM user WHERE email = @Email";
            cmd.Parameters.AddWithValue("@Email", email);
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var roleString = reader.GetString("userRoleId");
                int role = int.Parse(roleString);

                // Add logging to debug the roles
                Console.WriteLine($"Email: {email}, Role ID: {role}, Role: {(UserRole)role}");

                if (role == (int)UserRole.Customer)
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
                else if (role == (int)UserRole.Staff)
                {
                    return new Staff()
                    {
                        Id = reader.GetGuid("id"),
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password")
                    };
                }
            }
            return null;
        }
    }
}
