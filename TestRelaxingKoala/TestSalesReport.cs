using MySqlConnector;
using RelaxingKoala.Models.ViewModels;
using RelaxingKoala.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace TestRelaxingKoala
{
    public class TestSalesReport
    {
        private readonly SalesReportService _salesReportService;
        private readonly ITestOutputHelper output;
        public TestSalesReport(ITestOutputHelper output)
        {
            _salesReportService = new SalesReportService(new MySqlDataSource("Server=127.0.0.1;Port=3306;User ID=root;Password=admin;Database=rkdb"));
            this.output = output;
        }

        [Fact]
        public void TestGetMenuItemSalesByPeriod()
        {
            DateOnly startDate = new DateOnly(2002, 1, 1);
            DateOnly endDate = new DateOnly(2025, 12, 31);
            SalesReportViewModel sales = _salesReportService.GetMenuItemSalesByPeriod(2, startDate, endDate);
            output.WriteLine("{0} {1} {2}", sales.MenuItem.Name, sales.TotalSalesCost, sales.NumberOfSelection);
            foreach (var i in sales.Invoices)
            {
                output.WriteLine("{0} {1} {2}", i.TotalPay, i.GivenPay, i.ExcessPay);
            }
        }
    }
}
