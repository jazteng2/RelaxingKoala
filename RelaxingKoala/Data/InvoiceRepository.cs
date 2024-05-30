using MySqlConnector;
using RelaxingKoala.Models;

namespace RelaxingKoala.Data
{
    public class InvoiceRepository
    {
        private readonly MySqlDataSource _dataSource;
        public InvoiceRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public bool Insert(Invoice i)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO invoice (id, createdDate, totalPay, givenPay, excessPay, payMethodId, orderId, userId)
                VALUES (@id, @createdDate, @total, @given, @excess, @payMethod, @orderId, @userId)
            ";
            cmd.Parameters.AddWithValue("id", i.Id);
            cmd.Parameters.AddWithValue("createdDate", i.CreatedDate);
            cmd.Parameters.AddWithValue("total", i.TotalPay);
            cmd.Parameters.AddWithValue("given", i.GivenPay);
            cmd.Parameters.AddWithValue("excess", i.ExcessPay);
            cmd.Parameters.AddWithValue("payMethod", (int) i.PaymentMethod + 1);
            cmd.Parameters.AddWithValue("orderId", i.OrderId);
            cmd.Parameters.AddWithValue("userId", i.UserId);
            var affectedRows = cmd.ExecuteNonQuery();
            return affectedRows > 0 ? true : false; 
        }

        public Invoice GetById(Guid id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM invoice WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id);
            var reader = cmd.ExecuteReader();
            reader.Read();
            if (!reader.HasRows) return new Invoice();
            return new Invoice()
            {
                Id = reader.GetGuid("id"),
                CreatedDate = reader.GetDateOnly("createdDate"),
                TotalPay = reader.GetInt32("totalPay"),
                GivenPay = reader.GetInt32("givenpay"),
                ExcessPay = reader.GetInt32("excessPay"),
                PaymentMethod = GetPaymentMethod(reader.GetInt32("payMethodId")),
                UserId = reader.GetGuid("userId"),
                OrderId = reader.GetGuid("orderId")
            };
        }

        public Invoice GetByOrderId(Guid id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM invoice WHERE orderId = @orderId";
            cmd.Parameters.AddWithValue("orderId", id);
            var reader = cmd.ExecuteReader();
            reader.Read();
            if (!reader.HasRows) return new Invoice();
            return new Invoice()
            {
                IId = reader.GetGuid("id"),
                CreatedDate = reader.GetDateOnly("createdDate"),
                TotalPay = reader.GetInt32("totalPay"),
                GivenPay = reader.GetInt32("givenpay"),
                ExcessPay = reader.GetInt32("excessPay"),
                PaymentMethod = GetPaymentMethod(reader.GetInt32("payMethodId")),
                UserId = reader.GetGuid("userId"),
                OrderId = reader.GetGuid("orderId")
            };
        }
        
        public List<Invoice> GetInvoicesByUserId(Guid id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM invoice WHERE userId = @userId";
            cmd.Parameters.AddWithValue("userId", id);
            var reader = cmd.ExecuteReader();
            if (reader.HasRows) return new List<Invoice>();
            List<Invoice> list = new List<Invoice>();
            while (reader.Read())
            {
                list.Add(new Invoice()
                {
                    Id = reader.GetGuid("id"),
                    CreatedDate = reader.GetDateOnly("createdDate"),
                    TotalPay = reader.GetInt32("totalPay"),
                    GivenPay = reader.GetInt32("givenpay"),
                    ExcessPay = reader.GetInt32("excessPay"),
                    PaymentMethod = GetPaymentMethod(reader.GetInt32("payMethodId")),
                    UserId = reader.GetGuid("userId"),
                    OrderId = reader.GetGuid("orderId")
                });
            }
            return list;
        }

        public List<Invoice> GetByPeriod(DateOnly start, DateOnly end)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT * FROM invoice
                WHERE createdDate >= @startDate AND createdDate <= @endDate
            ";
            cmd.Parameters.AddWithValue("startDate", start);
            cmd.Parameters.AddWithValue("endDate", end);
            var reader = cmd.ExecuteReader();
            if (reader.HasRows) return new List<Invoice>();
            List<Invoice> list = new List<Invoice>();
            while (reader.Read())
            {
                list.Add(new Invoice()
                {
                    Id = reader.GetGuid("id"),
                    CreatedDate = reader.GetDateOnly("createdDate"),
                    TotalPay = reader.GetInt32("totalPay"),
                    GivenPay = reader.GetInt32("givenpay"),
                    ExcessPay = reader.GetInt32("excessPay"),
                    PaymentMethod = GetPaymentMethod(reader.GetInt32("payMethodId")),
                    UserId = reader.GetGuid("userId"),
                    OrderId = reader.GetGuid("orderId")
                });
            }
            return list;
        }
        public bool Update(Invoice i)
        {
            // Invoice exist because of an order therefore no update on orderId and userId
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE invoice SET 
                    totalPay = @totalPay,
                    givenPay = @givenPay,
                    excessPay = @excessPay,
                    payMethodId = @payMethod
            ";
            var affectedRow = cmd.ExecuteNonQuery();
            return affectedRow > 0 ? true : false;
        }

        public PaymentMethod GetPaymentMethod(int id)
        {
            using var conn = _dataSource.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM payMethod WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id);
            var reader = cmd.ExecuteReader();
            reader.Read();
            return (PaymentMethod)Enum.Parse(typeof(PaymentMethod), reader.GetString(1));
        }
    }
}
