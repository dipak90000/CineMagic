using System.Diagnostics;
using CineMagic.Models;
using CineMagic.Services;
using Microsoft.AspNetCore.Mvc;

namespace CineMagic.Controllers;

/// <summary>Home page (movie grid) and movie details / showtime picker.</summary>
public class HomeController : Controller
{
    /// <summary>Landing page: cinematic hero + "Now Showing" movie grid.</summary>
    public IActionResult Index()
    {
        return View(DataService.Movies);
    }

    /// <summary>Movie details page with an animated showtime picker.</summary>
    public IActionResult Movie(int id)
    {
        var movie = DataService.GetMovie(id);
        if (movie is null) return NotFound();

        // Group showtimes by date for the date-tab picker.
        var showtimes = DataService.GetShowtimes(id)
            .GroupBy(s => s.Date)
            .OrderBy(g => g.Key)
            .ToList();

        ViewData["ShowtimeGroups"] = showtimes;
        return View(movie);
    }

    /// <summary>All-movies catalogue with category filters and title search.</summary>
    public IActionResult Movies()
    {
        ViewData["Categories"] = DataService.Movies
            .Select(m => m.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
        return View(DataService.Movies);
    }

    /// <summary>Offers &amp; promotions page (Avengers: Doomsday early-bird hero).</summary>
    public IActionResult Offers()
    {
        var doomsday = DataService.Movies.FirstOrDefault(m => m.IsOffer);
        ViewData["OfferMovie"] = doomsday;
        return View(DataService.Movies.Where(m => m.IsOffer).ToList());
    }

    /// <summary>Contact page with a simple enquiry form + cinema info.</summary>
    [HttpGet]
    public IActionResult Contact()
    {
        return View(new ContactViewModel());
    }

    /// <summary>Handles the contact form (simulated send — shows a thank-you note).</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Contact(ContactViewModel form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        ViewData["ContactSent"] = true;
        return View(new ContactViewModel());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
