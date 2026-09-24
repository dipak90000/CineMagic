using System.ComponentModel.DataAnnotations;

namespace CineMagic.Models;

/// <summary>View-model for the checkout page (customer details + order summary).</summary>
public class CheckoutViewModel
{
    [Required(ErrorMessage = "Please tell us your name.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Name must be at least 2 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "We need your email for the e-ticket.")]
    [EmailAddress(ErrorMessage = "That doesn't look like a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your phone number.")]
    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit mobile number.")]
    public string Phone { get; set; } = string.Empty;

    // Carried through as hidden fields from the seat page.
    public string ShowtimeId { get; set; } = string.Empty;
    public string SeatsCsv { get; set; } = string.Empty;

    /// <summary>Optional promo code (e.g. DOOMSDAY20 for 20% off). Nullable so the
    /// field is genuinely optional — ASP.NET Core otherwise treats every
    /// non-nullable string as implicitly required.</summary>
    public string? PromoCode { get; set; }

    // Display-only, rebuilt server-side on GET and on validation failures.
    public Movie Movie { get; set; } = new();
    public Showtime Showtime { get; set; } = new();
    public List<SeatLine> SeatLines { get; set; } = new();

    /// <summary>Discount (₹) granted for the promo code, computed server-side.</summary>
    public decimal DiscountAmount { get; set; }

    public List<string> Seats =>
        SeatsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    public decimal Total => SeatLines.Sum(s => s.Price);

    /// <summary>Amount actually charged = seat total minus promo discount.</summary>
    public decimal GrandTotal => Total - DiscountAmount;
}

/// <summary>One booked seat with its price, for the order summary.</summary>
public class SeatLine
{
    public string Label { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
