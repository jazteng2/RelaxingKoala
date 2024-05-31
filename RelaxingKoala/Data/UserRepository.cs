using MySqlConnector;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.Users;
using System;
using System.Collections.Generic;

namespace RelaxingKoala.Data
{
    public class UserRepository
    {
        private readonly MySqlDataSource _dataSource;

        public UserRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public List<User> GetAll()
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM user";
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows) return new List<User>();
            List<User> users = new List<User>();
            while (reader.Read())
            {
                users.Add(GetUserObject(reader));
            }
            conn.Close();
            return users;
        }

        public User GetByEmail(string email)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM user WHERE email = @Email";
            cmd.Parameters.AddWithValue("@Email", email);
            var reader = cmd.ExecuteReader();
            if (!reader.Read()) return new Customer();
            return GetUserObject(reader);
        }

        public User GetById(Guid? id)
        {
            if (!id.HasValue) return new Customer();
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM user WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id.ToString());
            var reader = cmd.ExecuteReader();
            if (!reader.Read()) return new Customer();
            return GetUserObject(reader);
        }

        public bool Insert(User user)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO user (id, firstName, lastName, email, password, userRoleId)
                VALUES (@id, @firstName, @lastName, @email, @password, @userRoleId);
            ";
            cmd.Parameters.AddWithValue("id", user.Id.ToString());
            cmd.Parameters.AddWithValue("firstName", user.FirstName);
            cmd.Parameters.AddWithValue("lastName", user.LastName);
            cmd.Parameters.AddWithValue("email", user.Email);
            cmd.Parameters.AddWithValue("password", user.Password);
            cmd.Parameters.AddWithValue("userRoleId", (int)user.Role + 1);
            var affectedRows = cmd.ExecuteNonQuery();
            return affectedRows > 0;
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

        public User GetUserObject(MySqlDataReader reader)
        {
            var roleString = reader.GetString("userRoleId");
            if (!int.TryParse(roleString, out int role))
            {
                role = (int)UserRole.Customer; // Default to Customer if conversion fails
            }

            Guid userId = Guid.Parse(reader.GetString("id"));

            switch (role)
            {
                case (int)UserRole.Customer + 1:
                    return new Customer()
                    {
                        Id = userId,
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password"),
                        Role = GetRole(role)
                    };
                case (int)UserRole.Staff + 1:
                    return new Staff()
                    {
                        Id = userId,
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password"),
                        Role = GetRole(role)
                    };
                case (int)UserRole.Admin + 1:
                    return new Admin()
                    {
                        Id = userId,
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password"),
                        Role = GetRole(role)
                    };
                default:
                    return new Customer()
                    {
                        Id = userId,
                        FirstName = reader.GetString("firstName"),
                        LastName = reader.GetString("lastName"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password"),
                        Role = GetRole(role)
                    };
            }
        }
    }
}
