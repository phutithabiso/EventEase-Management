using EventEase_Management.Entity;
using EventEase_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EventEase_Management.Controllers
{
    public class VenueController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VenueController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult Venuepg()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddVenue(VenueModelView model, IFormFile ImageFile)
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

                var venueModel = new Venue
                {
                    Name = model.Name,
                    Location = model.Location,
                    Description = model.Description,
                    Capacity = model.Capacity,
                    Status = model.Status,
                    ImageUrl = model.ImageUrl,
                };

                try
                {
                    _context.Venues.Add(venueModel);
                    await _context.SaveChangesAsync();
                    ModelState.Clear();
                    ViewBag.Message = $"Venue '{venueModel.Name}' was successfully added!";
                    return RedirectToAction("Venuepg");
                }
                catch (DbUpdateException dbEx)
                {

                    Console.WriteLine($"Error occurred while adding venue: {dbEx.Message}");
                    ModelState.AddModelError("", "We cannot process your venue now, please try again later.");
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                }
            }

            return View("Venuepg", model);
        }


        public IActionResult Venuebook()
        {
            try
            {
                var venues = _context.Venues.ToList();

                if (venues == null || !venues.Any())
                {
                    ViewBag.Message = "No venues available.";
                }

                return View(venues);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error occurred while fetching venues: {ex.Message}");


                ViewBag.ErrorMessage = "Something went wrong while fetching the venue data.";
                return View(new List<Venue>());
            }
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            try
            {
                var v = await _context.Venues.FindAsync(id.Value);
                if (v == null)
                {
                    return NotFound();
                }

                return View(v);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error occurred while fetching venue details: {ex.Message}");

                ViewBag.ErrorMessage = "Something went wrong while fetching venue details.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var venue = _context.Venues.Find(id);
                if (venue == null)
                {
                    return NotFound();
                }


                var venueModel = new Venue
                {
                    VenueID = venue.VenueID,
                    Name = venue.Name,
                    Description = venue.Description,
                    Location = venue.Location,
                    Capacity = venue.Capacity,
                    Status = venue.Status,
                    ImageUrl = venue.ImageUrl
                };

                return View(venueModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while editing venue: {ex.Message}");
                ViewBag.ErrorMessage = "Something went wrong while editing the venue.";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VenueModelView model, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                var existingVenue = _context.Venues.Find(id);
                if (existingVenue == null)
                {
                    ModelState.AddModelError("", "Venue not found.");
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
                        ImageFile.CopyTo(fileStream);
                    }

                    existingVenue.ImageUrl = "/images/" + fileName;
                }

                existingVenue.Name = model.Name;
                existingVenue.Description = model.Description;
                existingVenue.Location = model.Location;
                existingVenue.Capacity = model.Capacity;
                existingVenue.Status = model.Status;


                try
                {
                    _context.Venues.Update(existingVenue);
                    _context.SaveChanges();
                    ViewBag.Message = $"Venue '{existingVenue.Name}' was successfully updated.";
                    return RedirectToAction("Venuebook");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "We cannot process your request now, please try again later.");
                }
            }

            return View(model);
        }


        // GET: Venue/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var venue = await _context.Venues.FirstOrDefaultAsync(v => v.VenueID == id.Value);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Delete Venue
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var venue = await _context.Venues.FindAsync(id);
                if (venue == null)
                {
                    return NotFound();
                }

                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Venuebook));
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error occurred while deleting venue: {ex.Message}");

                ViewBag.ErrorMessage = "An unexpected error occurred while deleting the venue.";
                return RedirectToAction(nameof(Venuebook));
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
