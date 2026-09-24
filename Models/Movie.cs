namespace CineMagic.Models;

/// <summary>A movie in the catalogue. Poster art is rendered with pure CSS
/// gradients + typography, so the site works fully offline.</summary>
public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public double Rating { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Cast { get; set; } = string.Empty;
    public string AgeRating { get; set; } = "U/A";
    public int ReleaseYear { get; set; }
    public string Tagline { get; set; } = string.Empty;

    /// <summary>Content bucket: "Bollywood", "Hollywood" or "South Cinema".</summary>
    public string Category { get; set; } = "Hollywood";

    /// <summary>Primary language, e.g. "Hindi", "English", "Telugu", "Tamil".</summary>
    public string Language { get; set; } = "English";

    /// <summary>Real poster image URL (TMDB CDN). The view falls back to the
    /// CSS gradient poster via onerror if the image can't load.</summary>
    public string BannerUrl { get; set; } = string.Empty;

    /// <summary>When true, the movie carries a special offer (promo code applies).</summary>
    public bool IsOffer { get; set; }

    /// <summary>CSS gradient used for the poster art, e.g. "linear-gradient(135deg,#f59e0b,#b45309)".</summary>
    public string PosterGradient { get; set; } = string.Empty;

    /// <summary>Decorative emoji / glyph shown on the poster.</summary>
    public string PosterIcon { get; set; } = "🎬";

    /// <summary>CSS gradient used for the backdrop on the details page.</summary>
    public string BackdropGradient { get; set; } = string.Empty;

    public string DurationText => $"{DurationMinutes / 60}h {DurationMinutes % 60}m";
}
