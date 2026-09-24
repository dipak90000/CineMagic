namespace CineMagic.Models;

/// <summary>View-model for the booking confirmation / e-ticket page.</summary>
public class ConfirmationViewModel
{
    public Booking Booking { get; set; } = new();
    public Movie Movie { get; set; } = new();
    public Showtime Showtime { get; set; } = new();
    public List<SeatLine> SeatLines { get; set; } = new();
}
