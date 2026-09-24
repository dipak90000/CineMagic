namespace CineMagic.Models;

/// <summary>One screening of a movie: a date, time, screen and format.</summary>
public class Showtime
{
    /// <summary>Deterministic id like "3-1-2" (movie-day-index).</summary>
    public string Id { get; set; } = string.Empty;
    public int MovieId { get; set; }
    public DateOnly Date { get; set; }
    public string Time { get; set; } = string.Empty;
    public string Screen { get; set; } = string.Empty;
    public string Format { get; set; } = "2D";

    /// <summary>Friendly label for the date tab: "Today", "Tomorrow" or "Sat, 27 Sep".</summary>
    public string DateLabel
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (Date == today) return "Today";
            if (Date == today.AddDays(1)) return "Tomorrow";
            return Date.ToString("ddd, dd MMM");
        }
    }

    public string DateTimeLabel => $"{DateLabel} • {Time}";
}
