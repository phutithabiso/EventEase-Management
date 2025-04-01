using EventEase_Management.Entity;
using EventEase_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEase_Management.Controllers
{

    public class EventController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var venues = _context.Venues.Select(v => new SelectListItem
            {
                Value = v.VenueID.ToString(),
                Text = v.Name
            }).ToList();

            var statuses = new List<SelectListItem>
        {
            new SelectListItem { Value = "Scheduled", Text = "Scheduled" },
            new SelectListItem { Value = "Completed", Text = "Completed" },
            new SelectListItem { Value = "Cancelled", Text = "Cancelled" }
        };

            ViewBag.VenueId = venues;
            ViewBag.Statuses = statuses;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEvent(EventModelView model, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileExtension = Path.GetExtension(ImageFile.FileName);
                    var fileName = Guid.NewGuid().ToString() + fileExtension;
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                    Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "images"));

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    model.ImageUrl = "/images/" + fileName;
                }

                var eventModel = new Event
                {
                    Name = model.Name,
                    Date = model.Date,
                    Description = model.Description,
                    Status = model.Status,
                    VenueId = model.VenueId,
                    //ImageUrl = model.ImageUrl,
                };

                try
                {
                    _context.Events.Add(eventModel);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"Event '{eventModel.Name}' was successfully added!";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException dbEx)
                {
                    Console.WriteLine($"Error occurred while adding event: {dbEx.Message}");
                    ModelState.AddModelError("", "We cannot process your request now, please try again later.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                }
            }

            return View("Index", model);
        }



        public IActionResult ListEvent()
        {
            try
            {
                var currentDate = DateTime.Now;
                var events = _context.Events.Include(e => e.Venues).ToList();

                var eventViewModel = new EventListModelView
                {
                    UpcomingEvents = events.Where(e => e.Date >= currentDate).ToList(),
                    PastEvents = events.Where(e => e.Date < currentDate).ToList()
                };

                return View(eventViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Something went wrong while fetching the event data.";
                return View(new EventListModelView());
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var eventEntity = _context.Events.Include(e => e.Venues).FirstOrDefault(ev => ev.EventID == id);
            if (eventEntity == null) return NotFound();

            var venues = _context.Venues.Select(v => new SelectListItem
            {
                Value = v.VenueID.ToString(),
                Text = v.Name
            }).ToList();

            ViewBag.VenueId = venues;

            var eventModel = new Event
            {
                EventID = eventEntity.EventID,
                Name = eventEntity.Name,
                Date = eventEntity.Date,
                Description = eventEntity.Description,
                Status = eventEntity.Status,
                VenueId = eventEntity.VenueId,
                ImageUrl = eventEntity.ImageUrl
            };

            return View(eventModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventModelView model, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
            {
                ModelState.AddModelError("", "Event not found.");
                return View(model);
            }

            // Handle image upload
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileExtension = Path.GetExtension(ImageFile.FileName);
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "images"));

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                existingEvent.ImageUrl = "/images/" + fileName;
            }


            existingEvent.Name = model.Name;
            existingEvent.Date = model.Date;
            existingEvent.Description = model.Description;
            existingEvent.Status = model.Status;
            existingEvent.VenueId = model.VenueId;

            try
            {
                _context.Events.Update(existingEvent);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Event '{existingEvent.Name}' was successfully updated.";
                return RedirectToAction("ListEvent");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "We cannot process your request now, please try again later.");
            }

            return View(model);
        }



        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var l = _context.Events.Include(e => e.Venues).FirstOrDefault(v => v.EventID == id);
            if (l == null) return NotFound();

            return View(l);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete == null) return NotFound();

            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListEvent));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return NotFound();

            var o = await _context.Events.Include(e => e.Venues).FirstOrDefaultAsync(ev => ev.EventID == id);
            if (o == null) return NotFound();

            return View(o);
        }
    }
}