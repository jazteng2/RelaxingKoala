using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using Xunit.Abstractions;
namespace TestRelaxingKoala
{
    public class TestTable
    {
        private readonly TableRepository repo;
        private readonly ITestOutputHelper output;
        public TestTable(ITestOutputHelper output)
        {
            repo = new TableRepository(new MySqlDataSource("Server=127.0.0.1;Port=3306;User ID=root;Password=admin;Database=rkdb"));
            this.output = output;
        }

        [Fact]
        public void TestFetchTables()
        {
            Assert.NotEmpty(repo.GetAll());
        }

        [Fact]
        public void TestFetchTable()
        {
            Table compare = new Table()
            {
                Id = 3,
                Number = 3,
                Availability = false
            };
            Table table = repo.GetById(3);
            output.WriteLine("table 3: {0} {1} {2}", table.Id, table.Number, table.Availability);
            Assert.Equal(compare.Id, table.Id);
        }

        [Fact]
        public void TestInsertTable()
        {
            Table table = new Table()
            {
                Number = 20,
                Availability = false
            };
            int result = repo.Insert(table);
            output.WriteLine("Id = {0}", result);
            Assert.Equal(10, result);
        }

        [Fact] void TestUpdateTable()
        {
            Table table = repo.GetById(10);
            if (table.Availability)
            {
                table.Availability = false;
            } else
            {
                table.Availability = true;
            }
            repo.Update(table);
            Table updated = repo.GetById(10);
            Assert.Equal(updated.Availability, table.Availability);
        }
    }
}