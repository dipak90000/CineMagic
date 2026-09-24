namespace CineMagic.Models;

/// <summary>View-model for the printable invoice page.</summary>
public class InvoiceViewModel
{
    public Booking Booking { get; set; } = new();
    public Movie Movie { get; set; } = new();
    public Showtime Showtime { get; set; } = new();
    public List<SeatLine> SeatLines { get; set; } = new();

    /// <summary>Human-friendly invoice number, e.g. INV-2026-8F3K2A.</summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>Seat subtotal before discount.</summary>
    public decimal Subtotal => SeatLines.Sum(s => s.Price);

    /// <summary>Promo discount granted (₹).</summary>
    public decimal Discount => Booking.DiscountAmount;

    /// <summary>Amount paid by the customer (subtotal − discount).</summary>
    public decimal GrandTotal => Booking.TotalAmount;

    /// <summary>
    /// GST shown as tax-inclusive (Indian cinema ticket prices include GST),
    /// so GST = grandTotal × 18 / 118.
    /// </summary>
    public decimal GstIncluded => Math.Round(GrandTotal * 18m / 118m, 2);
}
