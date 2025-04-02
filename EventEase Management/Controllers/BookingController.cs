using EventEase_Management.Entity;
using EventEase_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EventEase_Management.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult BookDetails(string searchTerm, string statusFilter)
        {
            try
            {
                var bookings = _context.Bookings
                    .Include(b => b.Events)
                    .Include(b => b.Venue)
                    .AsQueryable();


                if (!string.IsNullOrEmpty(searchTerm))
                {
                    bookings = bookings.Where(b =>
                        b.CustomerName.Contains(searchTerm) ||
                        b.Events.Name.Contains(searchTerm) ||
                        b.Status.Contains(searchTerm)
                    );
                }

                // Apply status filter
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    bookings = bookings.Where(b => b.Status == statusFilter);
                }

                ViewBag.StatusFilter = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "All" },
                new SelectListItem { Value = "Pending", Text = "Pending" },
                new SelectListItem { Value = "Confirmed", Text = "Confirmed" },
                new SelectListItem { Value = "Cancelled", Text = "Cancelled" }
            };

                ViewBag.EventId = new SelectList(_context.Events, "EventID", "Name");
                ViewBag.VenueId = new SelectList(_context.Venues, "VenueID", "Name");

                return View(bookings.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching bookings: " + ex.Message;
                return View(new List<Booking>());
            }
        }
        //GET Book
        public IActionResult Book()
        {
            try
            {
                ViewBag.EventId = new SelectList(_context.Events, "EventID", "Name");
                ViewBag.VenueId = new SelectList(_context.Venues, "VenueID", "Name");
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the booking form: " + ex.Message;
                return RedirectToAction("BookDetails");
            }
        }

        // Add a new booking (POST)
        
        // Edit booking view (GET)
        public IActionResult Edit(int id)
        {
            try
            {
                var booking = _context.Bookings
                    .Include(b => b.Events)
                    .Include(b => b.Venue)
                    .FirstOrDefault(b => b.BookingID == id);

                if (booking == null)
                {
                    return NotFound();
                }

                var model = new Booking
                {
                    BookingID = booking.BookingID,
                    CustomerName = booking.CustomerName,
                    EventID = booking.EventID,
                    VenueId = booking.VenueId,
                    BookingDate = booking.BookingDate,
                    Status = booking.Status
                };

                ViewBag.EventId = new SelectList(_context.Events, "EventID", "Name", model.EventID);
                ViewBag.VenueId = new SelectList(_context.Venues, "VenueID", "Name", model.VenueId);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the edit page: " + ex.Message;
                return RedirectToAction("BookDetails");
            }
        }

        // Edit booking (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit1(BookingModelView model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var booking = _context.Bookings.FirstOrDefault(b => b.BookingID == model.BookingID);

                    if (booking == null)
                    {
                        return NotFound();
                    }

                    booking.CustomerName = model.CustomerName;
                    booking.EventID = model.EventID;
                    booking.VenueId = model.VenueId;
                    booking.BookingDate = model.BookingDate;
                    booking.Status = model.Status;

                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Booking updated successfully!";
                    return RedirectToAction("BookDetails");
                }

                ViewBag.EventId = new SelectList(_context.Events, "EventID", "Name", model.EventID);
                ViewBag.VenueId = new SelectList(_context.Venues, "VenueID", "Name", model.VenueId);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the booking: " + ex.Message;
                return RedirectToAction("BookDetails");
            }
        }
        public IActionResult Delete()
        {
            return View();
        }
        //GET: Delete booking
        public IActionResult Delete(int id)
        {
            try
            {
                var booking = _context.Bookings.Find(id);
                if (booking == null)
                {
                    return NotFound();
                }

                _context.Bookings.Remove(booking);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Booking deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the booking: " + ex.Message;
            }

            return RedirectToAction("BookDetails");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
