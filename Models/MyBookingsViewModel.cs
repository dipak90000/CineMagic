using System.ComponentModel.DataAnnotations;

namespace CineMagic.Models;

/// <summary>View-model for the "My Bookings" lookup page.</summary>
public class MyBookingsViewModel
{
    /// <summary>Mobile number typed into the lookup form.</summary>
    [Required(ErrorMessage = "Please enter your mobile number to find your bookings.")]
    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit mobile number.")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>True once a lookup has been performed (so we can show an empty state).</summary>
    public bool Searched { get; set; }

    public List<MyBookingRow> Bookings { get; set; } = new();
}

/// <summary>One booking row with its resolved movie + showtime for display.</summary>
public class MyBookingRow
{
    public Booking Booking { get; set; } = new();
    public Movie Movie { get; set; } = new();
    public Showtime Showtime { get; set; } = new();
}
