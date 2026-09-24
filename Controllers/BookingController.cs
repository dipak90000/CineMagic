using CineMagic.Models;
using CineMagic.Services;
using Microsoft.AspNetCore.Mvc;

namespace CineMagic.Controllers;

/// <summary>Booking flow: Seats → Checkout → Confirmation → Invoice.</summary>
public class BookingController : Controller
{
    private readonly ISmsService _sms;
    private readonly ILogger<BookingController> _logger;

    public BookingController(ISmsService sms, ILogger<BookingController> logger)
    {
        _sms = sms;
        _logger = logger;
    }

    /// <summary>Interactive seat map for a chosen showtime.</summary>
    public IActionResult Seats(string showtimeId)
    {
        var showtime = DataService.GetShowtime(showtimeId);
        if (showtime is null) return RedirectToAction("Index", "Home");

        var movie = DataService.GetMovie(showtime.MovieId);
        if (movie is null) return RedirectToAction("Index", "Home");

        var sold = DataService.GetSoldSeats(showtimeId);

        var vm = new SeatMapViewModel
        {
            Movie = movie,
            Showtime = showtime,
            Rows = DataService.RowLabels.Select(row => new SeatRowViewModel
            {
                RowLabel = row.ToString(),
                Category = DataService.SeatCategory(row),
                Price = DataService.SeatPrice(row),
                Seats = Enumerable.Range(1, DataService.SeatsPerRow).Select(n => new SeatViewModel
                {
                    Label = $"{row}{n}",
                    IsSold = sold.Contains($"{row}{n}"),
                    Price = DataService.SeatPrice(row)
                }).ToList()
            }).ToList()
        };

        return View(vm);
    }

    /// <summary>Checkout form with order summary (seats passed via query string).</summary>
    [HttpGet]
    public IActionResult Checkout(string showtimeId, string seats)
    {
        var vm = BuildCheckoutViewModel(showtimeId, seats, new CheckoutViewModel());
        if (vm is null) return RedirectToAction("Index", "Home");
        return View(vm);
    }

    /// <summary>Processes payment (simulated) and creates the booking.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel form)
    {
        var showtime = DataService.GetShowtime(form.ShowtimeId);
        var seatList = form.Seats;

        if (showtime is null || seatList.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Your seat selection expired. Please pick seats again.");
        }

        // Guard against tampered / already-sold seats.
        var sold = showtime is null
            ? new HashSet<string>()
            : DataService.GetSoldSeats(showtime.Id);
        if (seatList.Any(s => sold.Contains(s)))
        {
            ModelState.AddModelError(string.Empty, "One or more of your seats was just booked by someone else. Please choose again.");
        }

        // Promo code is OPTIONAL and can never block checkout: an unrecognized
        // code is simply ignored (discount = 0) with a friendly notice shown
        // on the confirmation page. DOOMSDAY20 = 20% off, valid only on the
        // offer movie (Avengers: Doomsday). Always validated server-side.
        var promo = (form.PromoCode ?? string.Empty).Trim().ToUpperInvariant();
        var offerMovie = showtime is null ? null : DataService.GetMovie(showtime.MovieId);
        decimal discount = 0m;
        if (!string.IsNullOrEmpty(promo))
        {
            if (promo == "DOOMSDAY20" && offerMovie is { IsOffer: true })
            {
                discount = Math.Round(DataService.GetSeatLines(seatList).Sum(s => s.Price) * 0.20m, 2);
            }
            else
            {
                TempData["PromoNotice"] = $"Promo code '{promo}' wasn't recognized, so no discount was applied.";
                promo = string.Empty; // never store a bogus code on the booking
            }
        }

        if (!ModelState.IsValid)
        {
            var vm = BuildCheckoutViewModel(form.ShowtimeId, form.SeatsCsv, form);
            if (vm is null) return RedirectToAction("Index", "Home");
            return View(vm);
        }

        var booking = DataService.CreateBooking(
            form.Name.Trim(), form.Email.Trim(), form.Phone.Trim(),
            showtime!.MovieId, showtime.Id, seatList, promo, discount);

        // Best-effort SMS confirmation — wrapped so it can NEVER break the booking.
        try
        {
            var movieTitle = offerMovie?.Title ?? "your movie";
            var message = $"Your CineMagic booking {booking.Reference} for {movieTitle} is confirmed. " +
                          $"Seats: {string.Join(",", booking.Seats)}. Enjoy the show!";
            await _sms.SendBookingSmsAsync(booking.Phone, message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SMS notification failed for booking {Reference}", booking.Reference);
        }

        // PRG pattern: redirect to the confirmation page.
        return RedirectToAction(nameof(Confirmation), new { reference = booking.Reference });
    }

    /// <summary>E-ticket confirmation page with confetti.</summary>
    [HttpGet]
    public IActionResult Confirmation(string reference)
    {
        var booking = DataService.GetBooking(reference);
        if (booking is null) return RedirectToAction("Index", "Home");

        var movie = DataService.GetMovie(booking.MovieId);
        var showtime = DataService.GetShowtime(booking.ShowtimeId);
        if (movie is null || showtime is null) return RedirectToAction("Index", "Home");

        return View(new ConfirmationViewModel
        {
            Booking = booking,
            Movie = movie,
            Showtime = showtime,
            SeatLines = DataService.GetSeatLines(booking.Seats)
        });
    }

    /// <summary>Printable invoice for a completed booking (GST-inclusive).</summary>
    [HttpGet]
    public IActionResult Invoice(string reference)
    {
        var booking = DataService.GetBooking(reference);
        if (booking is null) return RedirectToAction("Index", "Home");

        var movie = DataService.GetMovie(booking.MovieId);
        var showtime = DataService.GetShowtime(booking.ShowtimeId);
        if (movie is null || showtime is null) return RedirectToAction("Index", "Home");

        return View(new InvoiceViewModel
        {
            Booking = booking,
            Movie = movie,
            Showtime = showtime,
            SeatLines = DataService.GetSeatLines(booking.Seats),
            InvoiceNumber = "INV-2026-" + booking.Reference.Replace("CM-", string.Empty)
        });
    }

    /// <summary>My bookings: look up past bookings by mobile number.</summary>
    [HttpGet]
    public IActionResult MyBookings(string? phone)
    {
        var vm = new MyBookingsViewModel();
        if (!string.IsNullOrWhiteSpace(phone))
        {
            vm.Phone = phone.Trim();
            vm.Searched = true;
            foreach (var b in DataService.GetBookingsByPhone(phone))
            {
                var movie = DataService.GetMovie(b.MovieId);
                var showtime = DataService.GetShowtime(b.ShowtimeId);
                if (movie is not null && showtime is not null)
                    vm.Bookings.Add(new MyBookingRow { Booking = b, Movie = movie, Showtime = showtime });
            }
        }
        return View(vm);
    }

    /// <summary>POST from the phone lookup form — redirects (PRG) to the GET.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MyBookingsLookup(string phone)
    {
        return RedirectToAction(nameof(MyBookings), new { phone = (phone ?? string.Empty).Trim() });
    }

    /// <summary>Cancels a booking (only if the reference belongs to the phone)
    /// and frees its seats so they become bookable again.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancel(string reference, string phone)
    {
        var booking = DataService.GetBooking(reference);
        var phoneDigits = DataService.DigitsOnly(phone);
        if (booking is null || DataService.DigitsOnly(booking.Phone) != phoneDigits || string.IsNullOrEmpty(phoneDigits))
        {
            TempData["MyBookingsError"] = "That booking wasn't found for this mobile number.";
        }
        else if (DataService.CancelBooking(reference))
        {
            TempData["MyBookingsSuccess"] = $"Booking {reference} is cancelled. Your seats are free again. 💺";
        }
        else
        {
            TempData["MyBookingsError"] = "Couldn't cancel that booking — it may already be gone.";
        }
        return RedirectToAction(nameof(MyBookings), new { phone = (phone ?? string.Empty).Trim() });
    }

    // Rebuilds the checkout view-model (display data) from the posted form.
    private static CheckoutViewModel? BuildCheckoutViewModel(string showtimeId, string seatsCsv, CheckoutViewModel form)
    {
        var showtime = DataService.GetShowtime(showtimeId);
        if (showtime is null) return null;
        var movie = DataService.GetMovie(showtime.MovieId);
        if (movie is null) return null;

        var seats = seatsCsv
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct()
            .Take(8)
            .ToList();
        if (seats.Count == 0) return null;

        form.ShowtimeId = showtimeId;
        form.SeatsCsv = string.Join(",", seats);
        form.Movie = movie;
        form.Showtime = showtime;
        form.SeatLines = DataService.GetSeatLines(seats);

        // Re-apply a valid promo for display (validation errors are handled by the caller).
        var promo = (form.PromoCode ?? string.Empty).Trim().ToUpperInvariant();
        form.PromoCode = promo;
        form.DiscountAmount = (promo == "DOOMSDAY20" && movie.IsOffer)
            ? Math.Round(form.Total * 0.20m, 2)
            : 0m;

        return form;
    }
}
