namespace CineMagic.Models;

/// <summary>A completed booking stored in the in-memory booking store.</summary>
public class Booking
{
    public string Reference { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int MovieId { get; set; }
    public string ShowtimeId { get; set; } = string.Empty;
    public List<string> Seats { get; set; } = new();
    public decimal TotalAmount { get; set; }

    /// <summary>Promo code applied at checkout (e.g. "DOOMSDAY20"), if any.</summary>
    public string PromoCode { get; set; } = string.Empty;

    /// <summary>Discount granted by the promo code (₹).</summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>Seat subtotal before discount (₹).</summary>
    public decimal SubtotalAmount { get; set; }
    public DateTime BookedAt { get; set; } = DateTime.Now;
}
