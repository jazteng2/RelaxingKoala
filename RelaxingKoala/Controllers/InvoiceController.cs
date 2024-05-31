using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Users;
using System.Security.Claims;

namespace RelaxingKoala.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly UserRepository userRepo;
        private readonly InvoiceRepository invoiceRepo;
        public InvoiceController(MySqlDataSource dataSource)
        {
            invoiceRepo = new InvoiceRepository(dataSource);
            userRepo = new UserRepository(dataSource);
        }
        public IActionResult Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            // Check role
            UserRole role = userRepo.GetById(new Guid(userId)).Role;
            List<Invoice> invoices;
            if (role == UserRole.Admin || role == UserRole.Staff)
            {
                invoices = invoiceRepo.GetAll();
            }
            else
            {
                invoices = invoiceRepo.GetInvoicesByUserId(new Guid(userId));
            }
            return View(invoices);
        }
    }
}
