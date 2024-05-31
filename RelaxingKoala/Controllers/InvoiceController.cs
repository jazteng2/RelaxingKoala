using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using System.Security.Claims;

namespace RelaxingKoala.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly InvoiceRepository invoiceRepo;
        public InvoiceController(MySqlDataSource dataSource)
        {
            invoiceRepo = new InvoiceRepository(dataSource);
        }
        public IActionResult Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            List<Invoice> invoices = invoiceRepo.GetInvoicesByUserId(new Guid(userId));
            return View(invoices);
        }
    }
}
