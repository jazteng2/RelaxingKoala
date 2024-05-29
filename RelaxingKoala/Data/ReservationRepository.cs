using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using RelaxingKoala.Models;

namespace RelaxingKoala.Data
{
    public class ReservationRepository
    {
        private readonly MySqlDataSource _dataSource;
        public ReservationRepository(MySqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public List<Reservation> GetAll()
        {
            // reservations
            var reservations = new List<Reservation>();
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();

            command.CommandText = @"SELECT * FROM reservation";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                reservations.Add(new Reservation()
                {
                    Id = reader.GetGuid("id"),
                    CreatedDate = DateOnly.FromDateTime(reader.GetDateTime("createdDate")),
                    ReservedDate = DateOnly.FromDateTime(reader.GetDateTime("reservedDate")),
                    StartTime = reader.GetTimeOnly("startTime"),
                    EndTime = reader.GetTimeOnly("endTime"),
                    NumberOfPeople = reader.GetInt32("numberofpeople"),
                    UserId = reader.GetGuid("userId")
                });
            }
            conn.Close();

            // table reservation associations
            using var conn2 = _dataSource.OpenConnection();
            using var command2 = conn2.CreateCommand();
            command2.CommandText = @"SELECT * FROM tablereservation";
            var reader2 = command2.ExecuteReader();
            while (reader2.Read())
            {
                var reservation = reservations.Find(r => r.Id == reader2.GetGuid("reservationId"));
                reservation.Tables.Add(reader2.GetInt32("tableId"));
            }
            return reservations;
        }

        public Reservation? GetById(Guid id)
        {
            // reservation
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"SELECT * FROM reservation WHERE id = @id";
            command.Parameters.AddWithValue("id", id);
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            var reservation = new Reservation()
            {
                Id = reader.GetGuid("id"),
                CreatedDate = DateOnly.FromDateTime(reader.GetDateTime("createdDate")),
                ReservedDate = DateOnly.FromDateTime(reader.GetDateTime("reservedDate")),
                StartTime = reader.GetTimeOnly("startTime"),
                EndTime = reader.GetTimeOnly("endTime"),
                NumberOfPeople = reader.GetInt32("numberofpeople"),
                UserId = reader.GetGuid("userId")
            };
            conn.Close();

            // table reservation association
            using var conn2 = _dataSource.OpenConnection();
            using var command2 = conn2.CreateCommand();
            command2.CommandText = @"SELECT t.id
                                    FROM dineintable t
                                    JOIN tablereservation rt ON t.id = rt.tableId
                                    WHERE rt.reservationId = @id";

            command2.Parameters.AddWithValue("id", reservation.Id);
            var reader2 = command2.ExecuteReader();
            while(reader2.Read())
            {
                reservation.Tables.Add(reader2.GetInt32("id"));
            }
            conn2.Close();
            return reservation;
        }

        public List<Table> GetReservedTables(Guid id)
        {
            List<Table> list = new List<Table>();
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"
                SELECT t.id, t.tablenumber, t.availability
                FROM dineintable t
                JOIN tablereservation rt ON t.id = rt.tableId
                WHERE rt.reservationId = @reservationId;
            ";
            command.Parameters.AddWithValue("reservationId", id);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Table
                {
                    Id = reader.GetInt32("id"),
                    Number = reader.GetInt32("tableNumber"),
                    Availability = reader.GetBoolean("availability")
                });
            }

            return list;
        }

        public void Insert(Reservation res)
        {
            // reservation
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"
                INSERT INTO Reservation (id, createdDate, reservedDate, startTime, endTime, numberOfPeople, userId)
                VALUES (@id, @createDate, @reservedDate, @startTime, @endTime, @numberOfPeople, @userId) 
            ";
            command.Parameters.AddWithValue("id", res.Id);
            command.Parameters.AddWithValue("createDate", res.CreatedDate);
            command.Parameters.AddWithValue("reservedDate", res.ReservedDate);
            command.Parameters.AddWithValue("startTime", res.StartTime);
            command.Parameters.AddWithValue("endTime", res.EndTime);
            command.Parameters.AddWithValue("numberOfPeople", res.NumberOfPeople);
            command.Parameters.AddWithValue("userId", res.UserId);
            command.ExecuteNonQuery();
            conn.Close();

            // table reservation association
            using var conn2 = _dataSource.OpenConnection();
            var tr = conn2.BeginTransaction();
            using var command2 = conn2.CreateCommand();
            command2.Transaction = tr;
            command2.CommandText = @"
                INSERT INTO tablereservation (tableId, reservationId)
                VALUES (@tbId, @rsId);
            ";
            foreach (var table in res.Tables)
            {
                command2.Parameters.Clear();
                command2.Parameters.AddWithValue("tbId", table);
                command2.Parameters.AddWithValue("rsId", res.Id);
                command2.ExecuteNonQuery();
            }

            tr.Commit();
            conn2.Close();
        }

        public void Delete(Guid id)
        {
            // table reservation association
            using var conn = _dataSource.OpenConnection();
            using var command = conn.CreateCommand();
            command.CommandText = @"DELETE FROM tablereservation WHERE reservationId = @id";
            command.Parameters.AddWithValue("id", id);
            command.ExecuteNonQuery();
            conn.Close();

            // reservation
            using var conn2 = _dataSource.OpenConnection();
            using var command2 = conn2.CreateCommand();
            command2.CommandText = @"DELETE FROM reservation WHERE id = @id";
            command2.Parameters.AddWithValue("id", id);
            command2.ExecuteNonQuery();
            conn2.Close();
        }
        
    }
}
