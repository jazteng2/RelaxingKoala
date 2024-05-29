using Microsoft.AspNetCore.Mvc;
using RelaxingKoala.Data;
using RelaxingKoala.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace RelaxingKoala.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ReservationRepository _reservationRepository;

        public ReservationsController(MySqlDataSource dataSource)
        {
            _reservationRepository = new ReservationRepository(dataSource);
        }

        public IActionResult Index()
        {
            var reservations = _reservationRepository.GetAll();
            return View(reservations);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,CreatedDate,ReservedDate,StartTime,EndTime,NumberOfPeople,UserId,Tables")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                reservation.Id = Guid.NewGuid();
                _reservationRepository.Insert(reservation);
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        public IActionResult Edit(Guid? id)
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
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(Guid id)
        {
            return _reservationRepository.Exists(id); // Implement Exists method in ReservationRepository if necessary
        }
    }
}
