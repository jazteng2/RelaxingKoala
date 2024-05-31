using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.ViewModels;

namespace RelaxingKoala.Services
{
    public class SalesReportService
    {
        private readonly OrderRepository orderRepo;
        private readonly InvoiceRepository invoiceRepo;
        private readonly MenuItemRepository menuItemRepo;
        public SalesReportService(MySqlDataSource dataSource)
        {
            orderRepo = new OrderRepository(dataSource);
            invoiceRepo = new InvoiceRepository(dataSource);
            menuItemRepo = new MenuItemRepository(dataSource);
        }

        public List<MenuItem> GetMenuItems()
        {
            return menuItemRepo.GetAll();
        }

        public List<Invoice> GetByPeriod(DateOnly startDate, DateOnly endDate)
        {
            return invoiceRepo.GetByPeriod(startDate, endDate);
        }
        
        public int GetMenuItemSalesByPeriod(Guid menuItemId, DateOnly startDate, DateOnly endDate)
        {
            int sales = 0;
            var invoices = GetByPeriod(startDate, endDate);
            
            foreach (var i in invoices)
            {
                if (i.Order.MenuItems.)
            }
        }
    }
}
