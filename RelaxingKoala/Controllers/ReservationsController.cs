using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using RelaxingKoala.Models.Users;
using System.Security.Claims;

namespace RelaxingKoala.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ReservationRepository reservationRepo;
        private readonly TableRepository tableRepo;
        private readonly UserRepository userRepo;
        public ReservationsController(MySqlDataSource dataSource)
        {
            reservationRepo = new ReservationRepository(dataSource);
            tableRepo = new TableRepository(dataSource);
            userRepo = new UserRepository(dataSource);
        }

        public IActionResult Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            UserRole role = userRepo.GetById(new Guid(userId)).Role;
            List<Reservation> reservations;
            if (role == UserRole.Admin || role == UserRole.Staff)
            {
                reservations = reservationRepo.GetAll();
            }
            else
            {
                reservations = reservationRepo.GetByUserId(new Guid(userId));
            }
            return View(reservations);
        }

        public IActionResult Create()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");
            ViewBag.Tables = reservationRepo.GetAvailableTables();
            return View(new Reservation() { UserId = new Guid(userId) });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ReservedDate,StartTime,EndTime,NumberOfPeople,UserId,Tables")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                reservation.Id = Guid.NewGuid();
                reservation.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
                reservationRepo.Insert(reservation);
                return RedirectToAction(nameof(Index), new { id = reservation.UserId });
            }
            ViewBag.Tables = reservationRepo.GetAvailableTables(); // Ensure ViewBag is set on postback
            return View(reservation);
        }

        public IActionResult Edit(Guid id)
        {
            var reservation = reservationRepo.GetById(id);
            if (reservation == null) return NotFound();

            ViewBag.Tables = reservationRepo.GetAvailableTables();
            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,CreatedDate,ReservedDate,StartTime,EndTime,NumberOfPeople,UserId,Tables")] Reservation reservation)
        {

            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    reservationRepo.Update(reservation); // Implement Update method in ReservationRepository if necessary
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!reservationRepo.Exists(reservation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        public IActionResult Delete(Guid? id)
        {
            if (id == null) return BadRequest();

            var reservation = reservationRepo.GetById(id.Value);
            if (reservation == null) return NotFound();

            return View(reservation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            reservationRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(Guid id)
        {
            return reservationRepo.Exists(id);
        }
    }
}