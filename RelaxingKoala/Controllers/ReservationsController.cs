using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using System.Security.Claims;

namespace RelaxingKoala.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ReservationRepository _reservationRepository;
        private readonly TableRepository _tableRepository;

        public ReservationsController(MySqlDataSource dataSource)
        {
            _reservationRepository = new ReservationRepository(dataSource);
            _tableRepository = new TableRepository(dataSource); 
        }
        public IActionResult MyReservations()
        {
            // Get user id
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            var reservations = _reservationRepository.GetByUserId(new Guid(userId));
            return View(reservations);
        }

        public IActionResult Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            List<Reservation> reservations;
            reservations = _reservationRepository.GetByUserId(new Guid(userId));
            return View(reservations);
        }

        public IActionResult Create(Guid? id)
        {
            if (id.HasValue)
            {
                ViewBag.Id = id.Value;
            }
            ViewBag.Tables = _reservationRepository.GetAvailableTables();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ReservedDate,StartTime,EndTime,NumberOfPeople,UserId,Tables")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                reservation.Id = Guid.NewGuid();
                reservation.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
                _reservationRepository.Insert(reservation);
                return RedirectToAction(nameof(Index), new { id = reservation.UserId });
            }
            ViewBag.Tables = _reservationRepository.GetAvailableTables(); // Ensure ViewBag is set on postback
            return View(reservation);
        }

        public IActionResult Edit(Guid id)
        {
            var reservation = _reservationRepository.GetById(id);
            if (reservation == null) return NotFound();

            ViewBag.Tables = _reservationRepository.GetAvailableTables();
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
                    _reservationRepository.Update(reservation); // Implement Update method in ReservationRepository if necessary
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_reservationRepository.Exists(reservation.Id))
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
            if (id == null)
            {
                return NotFound();
            }

            var reservation = _reservationRepository.GetById(id.Value);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _reservationRepository.Delete(id);
            return RedirectToAction(nameof(MyReservations));
        }

        private bool ReservationExists(Guid id)
        {
            return _reservationRepository.Exists(id); 
        }
    }
}