using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using System.Collections.Immutable;
using Xunit.Abstractions;

namespace TestRelaxingKoala
{
    public class TestReservation
    {
        private readonly ReservationRepository repo;
        private readonly ITestOutputHelper output;
        public TestReservation(ITestOutputHelper output)
        {
            repo = new ReservationRepository(new MySqlDataSource("Server=127.0.0.1;Port=3306;User ID=root;Password=admin;Database=rkdb"));
            this.output = output;
        }

        [Fact]
        public void TestFetchReservations()
        {
            List<Reservation> reservations = repo.GetAll();
            foreach (var r in reservations)
            {
                output.WriteLine(@"
                    {0}, {1}, {2}, {3}, {4}, {5}
                ", r.Id, r.CreatedDate, r.ReservedDate, r.StartTime, r.EndTime, r.NumberOfPeople, r.Tables.Count);

                foreach (var t in r.Tables)
                {
                    output.WriteLine(t.ToString());
                }
            }
        }

        [Fact] public void TestFetchReservation()
        {
            string id = "1f9f2c8f-659c-47d8-93b5-04d2c8f4312e";
            var reservation = repo.GetById(new Guid(id));
            var compare = new Reservation()
            {
                Id = new Guid(id),
                CreatedDate = new DateOnly(2024, 10, 10),
                ReservedDate = new DateOnly(2024, 10, 21),
                StartTime = new TimeOnly(18, 0),
                EndTime = new TimeOnly(20, 30),
                NumberOfPeople = 4,
            };
            output.WriteLine(compare.Id.ToString());
            output.WriteLine(reservation.Id.ToString());
            Assert.Equivalent(compare, reservation);
        }

        [Fact]
        public void TestFetchReservedTable()
        {
            var tables = repo.GetReservedTables(new Guid("1f9f2c8f-659c-47d8-93b5-04d2c8f4312e"));
            foreach (var t in tables)
            {
                output.WriteLine(@"
                    {0}, {1}, {2}
                ", t.Id, t.Number, t.Availability);
            }
        }

        [Fact]
        public void TestInsertReservation()
        {
            var reservation = new Reservation()
            {
                Id = Guid.NewGuid(),
                CreatedDate = new DateOnly(2024, 1, 1),
                ReservedDate = new DateOnly(2024, 2, 2),
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(12, 0),
                NumberOfPeople = 6,
                UserId = new Guid("e13b6f26-5f8d-4d94-87a2-b528c3b7e5df")
            };
            reservation.Tables.Add(1);
            reservation.Tables.Add(2);

            repo.Insert(reservation);
        }

        [Fact]
        public void TestDeleteReservation()
        {
            // Insert reservation
            var compare = new Reservation()
            {
                Id = Guid.NewGuid(),
                CreatedDate = new DateOnly(2024, 1, 1),
                ReservedDate = new DateOnly(2024, 2, 2),
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(12, 0),
                NumberOfPeople = 6,
                UserId = new Guid("e13b6f26-5f8d-4d94-87a2-b528c3b7e5df")
            };
            compare.Tables.Add(8);
            compare.Tables.Add(9);

            repo.Insert(compare);
            var res = repo.GetById(compare.Id);
            Assert.Equivalent(compare, res);

            // Delete reservation
            repo.Delete(compare.Id);
            Assert.Null(repo.GetById(compare.Id));
        }
    }
}
