using EventEase_Management.Entity;
using EventEase_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEase_Management.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                int totalBookings = _context.Bookings.Count();
                int activeEvents = _context.Events.Where(e => e.Date >= DateTime.Now).Count();

                // Get  venue IDs associated with either bookings or events
                var venueIdsWithBookingsOrEvents = _context.Bookings
                                                           .Select(b => b.VenueId)
                                                           .Union(_context.Events.Select(e => e.VenueId))
                                                           .Distinct()
                                                           .ToList();

                int venuesWithBookingsOrEvents = venueIdsWithBookingsOrEvents.Count();

                // Get recent bookings (last 5) with venue included
                var recentBookings = _context.Bookings
                                             .Include(b => b.Venue)
                                             .OrderByDescending(b => b.BookingDate)
                                             .Take(5)
                                             .ToList();

                // Get upcoming events (next 5) with venue included
                var upcomingEvents = _context.Events
                                             .Where(e => e.Date >= DateTime.Now)
                                             .Include(e => e.Venues)
                                             .OrderBy(e => e.Date)
                                             .Take(5)
                                             .ToList();

                var dashboardModel = new DashboardModelView
                {
                    TotalBookings = totalBookings,
                    ActiveEvents = activeEvents,
                    AvailableVenues = venuesWithBookingsOrEvents,
                    RecentBookings = recentBookings ?? new List<Booking>(),
                    UpcomingEvents = upcomingEvents ?? new List<Event>()
                };

                return View(dashboardModel);
            }
            catch (DbUpdateException dbEx)
            {
                ViewBag.ErrorMessage = "A database error occurred while fetching data. Please try again later.";
                Console.WriteLine($"Database error: {dbEx.Message}");
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while processing your request. Please try again later.";
                Console.WriteLine($"General error: {ex.Message}");
                return View("Error");
            }
        }


    }
}