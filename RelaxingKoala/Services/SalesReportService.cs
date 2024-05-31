using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Orders;
using RelaxingKoala.Models.ViewModels;
using System.Linq;

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

        public SalesReportViewModel GetMenuItemSalesByPeriod(int menuItemId, DateOnly startDate, DateOnly endDate)
        {
            int salesCost = 0;
            List<Invoice> correctInvoice = new List<Invoice>();
            MenuItem itemSelected = menuItemRepo.GetById(menuItemId);

            // Get invoices in period
            List<Invoice> invoicesByPeriod = GetByPeriod(startDate, endDate);

            // Get invoice where has menu item
            foreach (var i in invoicesByPeriod)
            {
                List<MenuItem> orderMenuItems = i.Order.MenuItems;
                var containItem = orderMenuItems.Where(m => m.Id == menuItemId);
                if (containItem.Any())
                {
                    correctInvoice.Add(i);
                }
            }

            salesCost += itemSelected.Cost * correctInvoice.Count();

            return new SalesReportViewModel()
            {
                MenuItem = itemSelected,
                Invoices = correctInvoice,
                TotalSalesCost = salesCost,
                NumberOfSelection = correctInvoice.Count()
            };
        }
    }
}
