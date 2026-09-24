namespace CineMagic.Models;

/// <summary>View-model for the interactive seat-map page.</summary>
public class SeatMapViewModel
{
    public Movie Movie { get; set; } = new();
    public Showtime Showtime { get; set; } = new();
    public List<SeatRowViewModel> Rows { get; set; } = new();
    public int MaxSeats { get; set; } = 8;
}

/// <summary>One row (A–H) of the seat map.</summary>
public class SeatRowViewModel
{
    public string RowLabel { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;   // Classic / Prime / Recliner
    public decimal Price { get; set; }
    public List<SeatViewModel> Seats { get; set; } = new();
}

/// <summary>A single seat in the map.</summary>
public class SeatViewModel
{
    public string Label { get; set; } = string.Empty;  // e.g. "C7"
    public bool IsSold { get; set; }
    public decimal Price { get; set; }
}
