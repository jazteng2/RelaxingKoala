using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace TestRelaxingKoala
{
    public class TestMenuItem
    {
        private readonly ITestOutputHelper output;
        private readonly MenuItemRepository _repo;
        public TestMenuItem(ITestOutputHelper output)
        {
            var conn = new MySqlDataSource("Server=127.0.0.1;Port=3306;User ID=root;Password=admin;Database=rkdb");
            _repo = new MenuItemRepository(conn);
            this.output = output;
        }

        [Fact]
        public void TestGetAll()
        {
            List<MenuItem> menuItems = _repo.GetAll();
            foreach (MenuItem item in menuItems)
            {
                DisplayItem(item);
            }
        }

        [Fact]
        public void TestGetById()
        {
            var item = _repo.GetById(1);
            DisplayItem(item);
        }

        private void DisplayItem(MenuItem m)
        {
            output.WriteLine("{0} {1} {2} {3}", m.Id, m.Name, m.Cost, m.Availability);
        }
    }
}
