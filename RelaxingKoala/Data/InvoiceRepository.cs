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
                INSERT INTO invoice (id, totalPay, givenPay, excessPay, payMethodId, orderId, userId)
                VALUES (@id, @total, @given, @excess, @payMethod, @orderId, @userId)
            ";
            cmd.Parameters.AddWithValue("id", i.Id);
            cmd.Parameters.AddWithValue("total", i.TotalPay);
            cmd.Parameters.AddWithValue("given", i.GivenPay);
            cmd.Parameters.AddWithValue("excess", i.ExcessPay);
            cmd.Parameters.AddWithValue("payMethod", (int) i.PaymentMethod + 1);
            cmd.Parameters.AddWithValue("orderId", i.OrderId);
            cmd.Parameters.AddWithValue("userId", i.UserId);
            var affectedRows = cmd.ExecuteNonQuery();
            return affectedRows > 0 ? true : false; 
        }
    }
}
