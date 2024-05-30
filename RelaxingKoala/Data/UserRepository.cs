using MySqlConnector;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.Users;
using System;

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


        public User GetById(Guid? id)
        {
            if (!id.HasValue) return new Customer();
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM user WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id);
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
                        Password = reader.GetString("password"),
                        Role = GetRole(reader.GetInt32("userRoleId"))
                    };
                case (int)UserRole.Staff + 1:
                    return new Staff()
                    {
                        Id = reader.GetGuid("id"),
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password"),
                        Role = GetRole(reader.GetInt32("userRoleId"))
                    };
                default:
                    return new Customer();
            }
        }
        public List<User> GetAll()
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM user";
            var reader = cmd.ExecuteReader();

            var users = new List<User>();
            while (reader.Read())
            {
                var roleString = reader.GetString("userRoleId");
                if (!int.TryParse(roleString, out int role))
                {
                    role = (int)UserRole.Customer; // Default to Customer if conversion fails
                }

                User user = role switch
                {
                    (int)UserRole.Customer => new Customer
                    {
                        Id = Guid.Parse(reader.GetString("id")),
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password"),
                        Role = GetRole(role)
                    },
                    (int)UserRole.Staff => new Staff
                    {
                        Id = Guid.Parse(reader.GetString("id")),
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password"),
                        Role = GetRole(role)
                    },
                    _ => new Customer()
                };

                users.Add(user);
            }

            return users;
        }


        public UserRole GetRole(int id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM userrole WHERE userroleid = @id";
            cmd.Parameters.AddWithValue("id", id);
            var reader = cmd.ExecuteReader();
            reader.Read();
            return (UserRole)Enum.Parse(typeof(UserRole), reader.GetString(1));
        }
    }
}
