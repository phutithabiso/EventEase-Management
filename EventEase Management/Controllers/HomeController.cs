using System.Diagnostics;
using EventEase_Management.Entity;
using EventEase_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEase_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                // Get the total number of bookings
                int totalBookings = _context.Bookings.Count();

                // Get the number of active events (those that are later than the current date)
                int activeEvents = _context.Events.Where(e => e.Date >= DateTime.Now).Count();

                // Get distinct venue IDs associated with bookings or events
                var venueIdsWithBookingsOrEvents = _context.Bookings
                    .Select(b => b.VenueId)
                    .Union(_context.Events.Select(e => e.VenueId))
                    .Distinct()
                    .ToList();

                // Count the number of venues with bookings or events
                int venuesWithBookingsOrEvents = venueIdsWithBookingsOrEvents.Count();

                // Get the most recent bookings 
                var recentBookings = _context.Bookings
                    .Include(b => b.Venue)
                    .OrderByDescending(b => b.BookingDate)
                    .Take(5)
                    .ToList();

                // Get the upcoming events (next 5)
                var upcomingEvents = _context.Events
                    .Where(e => e.Date >= DateTime.Now)
                    .Include(e => e.Venues)
                    .OrderBy(e => e.Date)
                    .Take(5)
                    .ToList();

                // Create the DashboardModelView with the gathered data
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
