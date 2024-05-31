using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;
using RelaxingKoala.Models.ViewModels;
using RelaxingKoala.Services;
using System.Security.Claims;

namespace RelaxingKoala.Controllers
{
    [Authorize]
    public class SalesReportController : Controller
    {
        private readonly SalesReportService salesReportService;
        public SalesReportController(MySqlDataSource dataSource)
        {
            salesReportService = new SalesReportService(dataSource);
        }
        [HttpGet]
        public IActionResult Index(SalesReportViewModel model)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            if (model == null) return View(new SalesReportViewModel());

            if (ModelState.IsValid)
            {
                SalesReportViewModel newModel = salesReportService.GetMenuItemSalesByPeriod(model.MenuItem.Id, model.StartDate, model.EndDate);
                return View(newModel);
            }
            return View(model);
        }
    }
}
