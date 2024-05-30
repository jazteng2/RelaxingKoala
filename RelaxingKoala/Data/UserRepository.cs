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

        public User GetByEmail(string id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM user WHERE email = @Email";
            cmd.Parameters.AddWithValue("@Email", id);
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

        public User GetById(Guid id)
        {
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

        private UserRole GetRole(int id)
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
