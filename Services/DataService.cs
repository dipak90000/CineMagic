using System.Collections.Concurrent;
using CineMagic.Models;

namespace CineMagic.Services;

/// <summary>
/// In-memory data layer: seeded movies, generated showtimes, deterministic
/// sold-seat maps and a booking store. No database needed — everything lives
/// in static collections, perfect for a demo you can run anywhere.
/// </summary>
public static class DataService
{
    // ------------------------------------------------------------------
    // Seat map configuration: 8 rows (A–H) × 10 seats, 3 price classes.
    // ------------------------------------------------------------------
    public static readonly char[] RowLabels = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
    public const int SeatsPerRow = 10;

    public static string SeatCategory(char row) => row switch
    {
        <= 'C' => "Classic",   // front rows
        <= 'F' => "Prime",     // middle rows
        _ => "Recliner"        // back rows
    };

    public static decimal SeatPrice(char row) => row switch
    {
        <= 'C' => 160m,
        <= 'F' => 220m,
        _ => 320m
    };

    // ------------------------------------------------------------------
    // Movies (poster art is pure CSS, so nothing can break offline).
    // ------------------------------------------------------------------
    public static readonly List<Movie> Movies = new()
    {
        new Movie
        {
            Id = 1, Title = "Dune: Part Two", Genre = "Sci-Fi • Adventure",
            DurationMinutes = 166, Rating = 8.8, ReleaseYear = 2024, Category = "Hollywood", Language = "English", AgeRating = "U/A 13+",
            Tagline = "Long live the fighters.",
            Description = "Paul Atreides unites with Chani and the Fremen while seeking revenge against the conspirators who destroyed his family — facing a choice between the love of his life and the fate of the known universe.",
            Cast = "Timothée Chalamet • Zendaya • Rebecca Ferguson",
            PosterIcon = "🏜️",
            BannerUrl = "https://image.tmdb.org/t/p/w500/6izwz7rsy95ARzTR3poZ8H6c5pp.jpg",
            PosterGradient = "linear-gradient(135deg, #c2410c 0%, #7c2d12 45%, #1c0a00 100%)",
            BackdropGradient = "radial-gradient(ellipse at 30% 20%, #9a3412 0%, transparent 60%), radial-gradient(ellipse at 80% 90%, #431407 0%, transparent 55%), #0a0503"
        },
        new Movie
        {
            Id = 2, Title = "Oppenheimer", Genre = "Biography • Drama",
            DurationMinutes = 180, Rating = 8.6, ReleaseYear = 2023, Category = "Hollywood", Language = "English", AgeRating = "A",
            Tagline = "The world forever changes.",
            Description = "The story of J. Robert Oppenheimer and the creation of the atomic bomb — a paradoxical man who must risk destroying the world in order to save it.",
            Cast = "Cillian Murphy • Emily Blunt • Matt Damon",
            PosterIcon = "💥",
            BannerUrl = "https://image.tmdb.org/t/p/w500/8Gxv8gSFCU0XGDykEGv7zR1n2ua.jpg",
            PosterGradient = "linear-gradient(135deg, #f59e0b 0%, #b45309 40%, #0c0a09 100%)",
            BackdropGradient = "radial-gradient(ellipse at 50% 30%, #b45309 0%, transparent 55%), radial-gradient(ellipse at 80% 80%, #7c2d12 0%, transparent 50%), #0a0805"
        },
        new Movie
        {
            Id = 3, Title = "Spider-Man: Across the Spider-Verse", Genre = "Animation • Action",
            DurationMinutes = 140, Rating = 8.9, ReleaseYear = 2023, Category = "Hollywood", Language = "English", AgeRating = "U/A",
            Tagline = "It's how you wear the mask that matters.",
            Description = "Miles Morales catapults across the Multiverse, where he encounters a team of Spider-People charged with protecting its very existence — and clashes with them over how to handle a new threat.",
            Cast = "Shameik Moore • Hailee Steinfeld • Oscar Isaac",
            PosterIcon = "🕷️",
            BannerUrl = "https://image.tmdb.org/t/p/w500/8Vt6mWEReuy4Of61Lnj5Xj704m8.jpg",
            PosterGradient = "linear-gradient(135deg, #dc2626 0%, #7f1d1d 40%, #0f0a1e 100%)",
            BackdropGradient = "radial-gradient(ellipse at 20% 20%, #dc2626 0%, transparent 55%), radial-gradient(ellipse at 85% 75%, #4c1d95 0%, transparent 55%), #0b0612"
        },
        new Movie
        {
            Id = 4, Title = "John Wick: Chapter 4", Genre = "Action • Thriller",
            DurationMinutes = 169, Rating = 8.2, ReleaseYear = 2023, Category = "Hollywood", Language = "English", AgeRating = "A",
            Tagline = "No way back. One way out.",
            Description = "With the price on his head ever increasing, legendary hit man John Wick uncovers a path to defeating the High Table — but a new enemy stands in his way.",
            Cast = "Keanu Reeves • Donnie Yen • Bill Skarsgård",
            PosterIcon = "🎯",
            BannerUrl = "https://image.tmdb.org/t/p/w500/vZloFAK7NmvMGKE7VkF5UHaz0I.jpg",
            PosterGradient = "linear-gradient(135deg, #0ea5e9 0%, #1e3a8a 45%, #020617 100%)",
            BackdropGradient = "radial-gradient(ellipse at 70% 25%, #0369a1 0%, transparent 55%), radial-gradient(ellipse at 20% 85%, #1e3a8a 0%, transparent 50%), #030610"
        },
        new Movie
        {
            Id = 5, Title = "The Batman", Genre = "Crime • Mystery",
            DurationMinutes = 176, Rating = 8.0, ReleaseYear = 2022, Category = "Hollywood", Language = "English", AgeRating = "U/A 16+",
            Tagline = "Unmask the truth.",
            Description = "Batman ventures into Gotham City's underworld when a sadistic killer leaves behind a trail of cryptic clues, forcing him to forge new relationships to bring the culprit to justice.",
            Cast = "Robert Pattinson • Zoë Kravitz • Paul Dano",
            PosterIcon = "🦇",
            BannerUrl = "https://image.tmdb.org/t/p/w500/74xTEgt7R36Fpooo50r9T25onhq.jpg",
            PosterGradient = "linear-gradient(135deg, #ef4444 0%, #450a0a 50%, #000000 100%)",
            BackdropGradient = "radial-gradient(ellipse at 50% 15%, #991b1b 0%, transparent 50%), radial-gradient(ellipse at 80% 90%, #1f1f23 0%, transparent 55%), #050505"
        },
        new Movie
        {
            Id = 6, Title = "Avatar: The Way of Water", Genre = "Sci-Fi • Fantasy",
            DurationMinutes = 192, Rating = 7.9, ReleaseYear = 2022, Category = "Hollywood", Language = "English", AgeRating = "U/A",
            Tagline = "Return to Pandora.",
            Description = "Jake Sully and Neytiri have formed a family on Pandora. When an ancient threat resurfaces, they must fight a difficult war against the humans and protect what matters most.",
            Cast = "Sam Worthington • Zoe Saldaña • Sigourney Weaver",
            PosterIcon = "🌊",
            BannerUrl = "https://image.tmdb.org/t/p/w500/t6HIqrRAclMCA60NsSmeqe9RmNV.jpg",
            PosterGradient = "linear-gradient(135deg, #22d3ee 0%, #0e7490 40%, #082f49 100%)",
            BackdropGradient = "radial-gradient(ellipse at 30% 70%, #0e7490 0%, transparent 55%), radial-gradient(ellipse at 75% 20%, #155e75 0%, transparent 50%), #04121d"
        },
        new Movie
        {
            Id = 7, Title = "Jawan", Genre = "Action • Thriller",
            DurationMinutes = 169, Rating = 7.8, ReleaseYear = 2023, Category = "Bollywood", Language = "Hindi", AgeRating = "U/A",
            Tagline = "Ready.",
            Description = "A high-octane action thriller where a vigilante jailer and his army of women take on a corrupt system — with a father-son duo at the heart of the storm.",
            Cast = "Shah Rukh Khan • Nayanthara • Vijay Sethupathi",
            PosterIcon = "🔥",
            BannerUrl = "https://image.tmdb.org/t/p/w500/jFt1gS4BGHlK8xt76Y81Alp4dbt.jpg",
            PosterGradient = "linear-gradient(135deg, #f97316 0%, #991b1b 45%, #1c0a00 100%)",
            BackdropGradient = "radial-gradient(ellipse at 25% 25%, #ea580c 0%, transparent 55%), radial-gradient(ellipse at 80% 80%, #7f1d1d 0%, transparent 50%), #120602"
        },
        new Movie
        {
            Id = 8, Title = "RRR", Genre = "Action • Drama",
            DurationMinutes = 182, Rating = 8.4, ReleaseYear = 2022, Category = "South Cinema", Language = "Telugu", AgeRating = "U/A",
            Tagline = "Rise. Roar. Revolt.",
            Description = "A fictitious story about two legendary revolutionaries and their journey away from home before they started fighting for their country in the 1920s.",
            Cast = "N. T. Rama Rao Jr. • Ram Charan • Alia Bhatt",
            PosterIcon = "⚔️",
            BannerUrl = "https://image.tmdb.org/t/p/w500/u0XUBNQWlOvrh0Gd97ARGpIkL0.jpg",
            PosterGradient = "linear-gradient(135deg, #eab308 0%, #a16207 40%, #1c1005 100%)",
            BackdropGradient = "radial-gradient(ellipse at 60% 20%, #a16207 0%, transparent 55%), radial-gradient(ellipse at 15% 85%, #713f12 0%, transparent 50%), #100a02"
        },
        new Movie
        {
            Id = 9, Title = "Pathaan", Genre = "Action • Thriller",
            DurationMinutes = 146, Rating = 7.1, ReleaseYear = 2023, Category = "Bollywood", Language = "Hindi", AgeRating = "U/A",
            Tagline = "The countdown begins.",
            Description = "An Indian agent races against a doomsday clock as a ruthless mercenary, backed by a secret terror outfit, plans a devastating attack on India.",
            Cast = "Shah Rukh Khan • Deepika Padukone • John Abraham",
            PosterIcon = "✈️",
            BannerUrl = "https://image.tmdb.org/t/p/w500/arf00BkwvXo0CFKbaD9OpqdE4Nu.jpg",
            PosterGradient = "linear-gradient(135deg, #f97316 0%, #7c2d12 45%, #120602 100%)",
            BackdropGradient = "radial-gradient(ellipse at 30% 25%, #c2410c 0%, transparent 55%), radial-gradient(ellipse at 80% 80%, #431407 0%, transparent 50%), #0f0602"
        },
        new Movie
        {
            Id = 10, Title = "Animal", Genre = "Crime • Drama",
            DurationMinutes = 201, Rating = 7.2, ReleaseYear = 2023, Category = "Bollywood", Language = "Hindi", AgeRating = "A",
            Tagline = "A father's love. A son's rage.",
            Description = "The hardened son of a powerful industrialist returns home after years abroad and vows to take bloody revenge on those threatening his father's life.",
            Cast = "Ranbir Kapoor • Rashmika Mandanna • Anil Kapoor",
            PosterIcon = "🐺",
            BannerUrl = "https://image.tmdb.org/t/p/w500/hr9rjR3J0xBBKmlJ4n3gHId9ccx.jpg",
            PosterGradient = "linear-gradient(135deg, #991b1b 0%, #450a0a 50%, #0a0202 100%)",
            BackdropGradient = "radial-gradient(ellipse at 50% 30%, #7f1d1d 0%, transparent 55%), radial-gradient(ellipse at 20% 85%, #450a0a 0%, transparent 50%), #0c0303"
        },
        new Movie
        {
            Id = 11, Title = "Pushpa 2: The Rule", Genre = "Action • Crime",
            DurationMinutes = 200, Rating = 7.6, ReleaseYear = 2024, Category = "South Cinema", Language = "Telugu", AgeRating = "U/A",
            Tagline = "The rule continues.",
            Description = "Pushpa Raj's smuggling empire grows beyond borders as old enemies return — and the rule of Pushpa faces its fiercest test yet.",
            Cast = "Allu Arjun • Rashmika Mandanna • Fahadh Faasil",
            PosterIcon = "🪓",
            BannerUrl = "https://image.tmdb.org/t/p/w500/1T21FblunT0y8fz7YaW8JMYgUKm.jpg",
            PosterGradient = "linear-gradient(135deg, #b45309 0%, #713f12 45%, #140b02 100%)",
            BackdropGradient = "radial-gradient(ellipse at 65% 20%, #b45309 0%, transparent 55%), radial-gradient(ellipse at 15% 80%, #451a03 0%, transparent 50%), #100902"
        },
        new Movie
        {
            Id = 12, Title = "Kalki 2898 AD", Genre = "Sci-Fi • Mythology",
            DurationMinutes = 181, Rating = 7.8, ReleaseYear = 2024, Category = "South Cinema", Language = "Telugu", AgeRating = "U/A",
            Tagline = "The future is written in the past.",
            Description = "In a post-apocalyptic world in the year 2898 AD, a modern-day avatar of Vishnu descends to earth to protect the world from evil forces.",
            Cast = "Prabhas • Deepika Padukone • Amitabh Bachchan",
            PosterIcon = "🛸",
            BannerUrl = "https://image.tmdb.org/t/p/w500/rstcAnBeCkxNQjNp3YXrF6IP1tW.jpg",
            PosterGradient = "linear-gradient(135deg, #f59e0b 0%, #92400e 40%, #0c0902 100%)",
            BackdropGradient = "radial-gradient(ellipse at 40% 25%, #d97706 0%, transparent 55%), radial-gradient(ellipse at 85% 80%, #451a03 0%, transparent 50%), #0e0802"
        },
        new Movie
        {
            Id = 13, Title = "Leo", Genre = "Action • Thriller",
            DurationMinutes = 164, Rating = 7.2, ReleaseYear = 2023, Category = "South Cinema", Language = "Tamil", AgeRating = "U/A",
            Tagline = "A calm man. A violent past.",
            Description = "A mild-mannered café owner becomes a local hero through an act of violence, which sets off repercussions that shake his life — as a drug cartel claims he is one of their own.",
            Cast = "Vijay • Sanjay Dutt • Trisha",
            PosterIcon = "☕",
            BannerUrl = "https://image.tmdb.org/t/p/w500/2XUHC4lp3tDsgfFLFygNZ2x2Um9.jpg",
            PosterGradient = "linear-gradient(135deg, #0ea5e9 0%, #1e3a8a 45%, #020617 100%)",
            BackdropGradient = "radial-gradient(ellipse at 70% 25%, #0369a1 0%, transparent 55%), radial-gradient(ellipse at 20% 85%, #172554 0%, transparent 50%), #030610"
        },
        new Movie
        {
            Id = 14, Title = "Jailer", Genre = "Action • Comedy",
            DurationMinutes = 165, Rating = 7.3, ReleaseYear = 2023, Category = "South Cinema", Language = "Tamil", AgeRating = "U/A",
            Tagline = "No jail can hold him.",
            Description = "A retired jailer goes on a manhunt to find his son's killers — but the road leads him to a deeper, darker secret.",
            Cast = "Rajinikanth • Vinayakan • Ramya Krishnan",
            PosterIcon = "🔒",
            BannerUrl = "https://image.tmdb.org/t/p/w500/pTmMxAHqX4vsIDE6HPPxOR0Q6TN.jpg",
            PosterGradient = "linear-gradient(135deg, #64748b 0%, #1e293b 50%, #020617 100%)",
            BackdropGradient = "radial-gradient(ellipse at 30% 30%, #475569 0%, transparent 55%), radial-gradient(ellipse at 80% 80%, #0f172a 0%, transparent 50%), #05070d"
        },
        new Movie
        {
            Id = 15, Title = "Avengers: Doomsday", Genre = "Action • Adventure",
            DurationMinutes = 150, Rating = 9.1, ReleaseYear = 2026, Category = "Hollywood", Language = "English", AgeRating = "U/A 13+",
            IsOffer = true,
            Tagline = "Doom is coming.",
            Description = "The most anticipated event in cinema history. Earth's mightiest heroes face Doctor Doom as the multiverse collapses — early-bird bookings get flat 20% off with code DOOMSDAY20.",
            Cast = "Robert Downey Jr. • The Avengers assemble",
            PosterIcon = "⚡",
            BannerUrl = "https://image.tmdb.org/t/p/w500/jzPwsojjFStf5lR5Nm07w2hH56G.jpg",
            PosterGradient = "linear-gradient(135deg, #8b5cf6 0%, #4c1d95 45%, #0b0614 100%)",
            BackdropGradient = "radial-gradient(ellipse at 50% 20%, #7c3aed 0%, transparent 55%), radial-gradient(ellipse at 85% 85%, #2e1065 0%, transparent 50%), #0a0612"
        },
        new Movie
        {
            Id = 16, Title = "Stree 2: Sarkate Ka Aatank", Genre = "Horror • Comedy",
            DurationMinutes = 149, Rating = 8.1, ReleaseYear = 2024, Category = "Bollywood", Language = "Hindi", AgeRating = "U/A",
            Tagline = "O Stree, raksha karna!",
            Description = "The town of Chanderi is haunted again — this time, women are being abducted by the terrifying headless entity Sarkata. Vicky and his gang must team up with Stree herself to save the town.",
            Cast = "Shraddha Kapoor • Rajkummar Rao • Pankaj Tripathi",
            PosterIcon = "👻",
            BannerUrl = "https://image.tmdb.org/t/p/w500/2NC7sj8rheKxWqLYAbHnCa4mYBH.jpg",
            PosterGradient = "linear-gradient(135deg, #7c3aed 0%, #4c1d95 45%, #0b0614 100%)",
            BackdropGradient = "radial-gradient(ellipse at 30% 25%, #6d28d9 0%, transparent 55%), radial-gradient(ellipse at 80% 80%, #2e1065 0%, transparent 50%), #0d0716"
        },
        new Movie
        {
            Id = 17, Title = "Fighter", Genre = "Action • Drama",
            DurationMinutes = 166, Rating = 7.4, ReleaseYear = 2024, Category = "Bollywood", Language = "Hindi", AgeRating = "U/A",
            Tagline = "Top guns. Higher stakes.",
            Description = "India's first aerial action franchise: top IAF aviators form the Air Dragons squadron and take on a deadly terror threat, battling enemies in the sky and demons within.",
            Cast = "Hrithik Roshan • Deepika Padukone • Anil Kapoor",
            PosterIcon = "✈️",
            BannerUrl = "https://image.tmdb.org/t/p/w500/cQ5u9RcrUEGGlnHvMnHf96rLYgD.jpg",
            PosterGradient = "linear-gradient(135deg, #38bdf8 0%, #1d4ed8 45%, #0a1128 100%)",
            BackdropGradient = "radial-gradient(ellipse at 60% 20%, #0284c7 0%, transparent 55%), radial-gradient(ellipse at 20% 85%, #1e3a8a 0%, transparent 50%), #060d1f"
        },
        new Movie
        {
            Id = 18, Title = "Shaitaan", Genre = "Horror • Thriller",
            DurationMinutes = 132, Rating = 7.0, ReleaseYear = 2024, Category = "Bollywood", Language = "Hindi", AgeRating = "U/A 16+",
            Tagline = "Evil has a new face.",
            Description = "A family's weekend getaway turns into a nightmare when a mysterious stranger takes control of their daughter through black magic — and the parents must confront pure evil to save her.",
            Cast = "Ajay Devgn • R. Madhavan • Jyotika",
            PosterIcon = "😈",
            BannerUrl = "https://image.tmdb.org/t/p/w500/oRvFzcagAcC6Q317xtV7QXzwBnj.jpg",
            PosterGradient = "linear-gradient(135deg, #7f1d1d 0%, #450a0a 50%, #080202 100%)",
            BackdropGradient = "radial-gradient(ellipse at 50% 30%, #991b1b 0%, transparent 55%), radial-gradient(ellipse at 20% 85%, #450a0a 0%, transparent 50%), #0a0303"
        },
        new Movie
        {
            Id = 19, Title = "Dunki", Genre = "Comedy • Drama",
            DurationMinutes = 161, Rating = 7.2, ReleaseYear = 2023, Category = "Bollywood", Language = "Hindi", AgeRating = "U/A",
            Tagline = "Dreams have no borders.",
            Description = "Rajkumar Hirani's heartwarming tale of four friends from Punjab who dream of a better life in London and take the dangerous 'donkey flight' route to get there.",
            Cast = "Shah Rukh Khan • Taapsee Pannu • Vicky Kaushal",
            PosterIcon = "🌍",
            BannerUrl = "https://image.tmdb.org/t/p/w500/18IWZJQUg9iN5Bi7AjSA2R0WRel.jpg",
            PosterGradient = "linear-gradient(135deg, #f59e0b 0%, #b45309 45%, #1c1005 100%)",
            BackdropGradient = "radial-gradient(ellipse at 40% 30%, #d97706 0%, transparent 55%), radial-gradient(ellipse at 80% 80%, #78350f 0%, transparent 50%), #140c04"
        },
        new Movie
        {
            Id = 20, Title = "Salaar: Part 1 – Ceasefire", Genre = "Action • Thriller",
            DurationMinutes = 175, Rating = 7.5, ReleaseYear = 2023, Category = "South Cinema", Language = "Telugu", AgeRating = "A",
            Tagline = "The most violent man.",
            Description = "In the fictional city-state of Khansaar, tribesman Deva and prince Varadha's friendship is tested when a coup is planned — the KGF director and Baahubali star unite for an epic saga.",
            Cast = "Prabhas • Prithviraj Sukumaran • Shruti Haasan",
            PosterIcon = "🗡️",
            BannerUrl = "https://image.tmdb.org/t/p/w500/wbonIVQaGmtUkZnlPIqpUBCIFdy.jpg",
            PosterGradient = "linear-gradient(135deg, #475569 0%, #1e293b 50%, #020617 100%)",
            BackdropGradient = "radial-gradient(ellipse at 55% 25%, #334155 0%, transparent 55%), radial-gradient(ellipse at 20% 85%, #0f172a 0%, transparent 50%), #04070f"
        },
        new Movie
        {
            Id = 21, Title = "12th Fail", Genre = "Biography • Drama",
            DurationMinutes = 147, Rating = 8.7, ReleaseYear = 2023, Category = "Bollywood", Language = "Hindi", AgeRating = "U",
            Tagline = "Restart.",
            Description = "The inspiring true story of Manoj Kumar Sharma, who overcomes extreme poverty and repeated failure to crack the UPSC — a love letter to every dreamer who refuses to give up.",
            Cast = "Vikrant Massey • Medha Shankr",
            PosterIcon = "📚",
            BannerUrl = "https://image.tmdb.org/t/p/w500/cDWW5l4NTWtQi9McwevrY3knsTd.jpg",
            PosterGradient = "linear-gradient(135deg, #eab308 0%, #a16207 45%, #1c1005 100%)",
            BackdropGradient = "radial-gradient(ellipse at 35% 30%, #ca8a04 0%, transparent 55%), radial-gradient(ellipse at 80% 80%, #713f12 0%, transparent 50%), #120c03"
        },
        new Movie
        {
            Id = 22, Title = "Deadpool & Wolverine", Genre = "Action • Comedy",
            DurationMinutes = 128, Rating = 8.0, ReleaseYear = 2024, Category = "Hollywood", Language = "English", AgeRating = "A",
            Tagline = "Come together.",
            Description = "The merc with a mouth teams up with the grumpiest mutant alive for a multiverse-hopping mission that will change the MCU forever. Maximum effort, maximum chaos.",
            Cast = "Ryan Reynolds • Hugh Jackman • Emma Corrin",
            PosterIcon = "🔥",
            BannerUrl = "https://image.tmdb.org/t/p/w500/8cdWjvZQUExUUTzyp4t6EDMubfO.jpg",
            PosterGradient = "linear-gradient(135deg, #dc2626 0%, #facc15 45%, #1c0a00 100%)",
            BackdropGradient = "radial-gradient(ellipse at 30% 25%, #b91c1c 0%, transparent 55%), radial-gradient(ellipse at 80% 80%, #a16207 0%, transparent 50%), #120802"
        },
        new Movie
        {
            Id = 23, Title = "Inside Out 2", Genre = "Animation • Family",
            DurationMinutes = 96, Rating = 7.9, ReleaseYear = 2024, Category = "Hollywood", Language = "English", AgeRating = "U",
            Tagline = "New feelings. New chaos.",
            Description = "Riley hits puberty and brand-new emotions — Anxiety, Envy, Ennui and Embarrassment — storm headquarters, turning Joy and the crew's world upside down.",
            Cast = "Amy Poehler • Maya Hawke • Kensington Tallman",
            PosterIcon = "🧠",
            BannerUrl = "https://image.tmdb.org/t/p/w500/vpnVM9B6NMmQpWeZvzLvDESb2QY.jpg",
            PosterGradient = "linear-gradient(135deg, #fb923c 0%, #0d9488 55%, #0c1a17 100%)",
            BackdropGradient = "radial-gradient(ellipse at 40% 25%, #ea580c 0%, transparent 55%), radial-gradient(ellipse at 80% 80%, #0f766e 0%, transparent 50%), #0e100d"
        }
    };

    public static Movie? GetMovie(int id) => Movies.FirstOrDefault(m => m.Id == id);

    // ------------------------------------------------------------------
    // Showtimes: 3 days × several screens/times per movie, generated
    // deterministically so they look the same on every run.
    // ------------------------------------------------------------------
    private static readonly (string Time, string Screen, string Format)[] DailySlots =
    {
        ("10:30 AM", "SCREEN 2", "2D"),
        ("01:45 PM", "SCREEN 1", "2D"),
        ("05:15 PM", "IMAX", "IMAX 2D"),
        ("09:00 PM", "SCREEN 3", "4DX"),
    };

    public static List<Showtime> GetShowtimes(int movieId)
    {
        var list = new List<Showtime>();
        for (var day = 0; day < 3; day++)
        {
            for (var i = 0; i < DailySlots.Length; i++)
            {
                var slot = DailySlots[i];
                list.Add(new Showtime
                {
                    Id = $"{movieId}-{day}-{i}",
                    MovieId = movieId,
                    Date = DateOnly.FromDateTime(DateTime.Today.AddDays(day)),
                    Time = slot.Time,
                    Screen = slot.Screen,
                    Format = slot.Format
                });
            }
        }
        return list;
    }

    public static Showtime? GetShowtime(string id)
    {
        var parts = id.Split('-');
        if (parts.Length != 3) return null;
        if (!int.TryParse(parts[0], out var movieId)) return null;
        return GetShowtimes(movieId).FirstOrDefault(s => s.Id == id);
    }

    // ------------------------------------------------------------------
    // Sold seats: deterministic pseudo-random per showtime so the map
    // always looks realistically "already booked" (~25% sold).
    // ------------------------------------------------------------------
    public static HashSet<string> GetSoldSeats(string showtimeId)
    {
        var sold = new HashSet<string>();
        var rng = new Random(showtimeId.GetHashCode());
        foreach (var row in RowLabels)
        {
            for (var n = 1; n <= SeatsPerRow; n++)
            {
                if (rng.NextDouble() < 0.25)
                    sold.Add($"{row}{n}");
            }
        }
        // Plus seats taken by real bookings made in this session.
        if (BookedSeats.TryGetValue(showtimeId, out var taken))
        {
            lock (taken) sold.UnionWith(taken);
        }
        return sold;
    }

    public static List<SeatLine> GetSeatLines(IEnumerable<string> seatLabels)
    {
        return seatLabels.Select(label => new SeatLine
        {
            Label = label,
            Category = SeatCategory(label[0]),
            Price = SeatPrice(label[0])
        }).ToList();
    }

    // ------------------------------------------------------------------
    // Booking store (in-memory). Thread-safe dictionary keyed by reference.
    // BookedSeats tracks seats taken by real bookings per showtime so the
    // seat map (and the "already sold" guard) stay truthful in-session.
    // ------------------------------------------------------------------
    private static readonly ConcurrentDictionary<string, Booking> Bookings = new();
    private static readonly ConcurrentDictionary<string, HashSet<string>> BookedSeats = new();

    /// <summary>Keeps only digits — used to compare phone numbers robustly.</summary>
    public static string DigitsOnly(string? phone) =>
        new((phone ?? string.Empty).Where(char.IsDigit).ToArray());

    public static Booking CreateBooking(string name, string email, string phone,
        int movieId, string showtimeId, List<string> seats,
        string promoCode = "", decimal discountAmount = 0m)
    {
        var seatLines = GetSeatLines(seats);
        var subtotal = seatLines.Sum(s => s.Price);
        var booking = new Booking
        {
            Reference = "CM-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant(),
            CustomerName = name,
            Email = email,
            Phone = phone,
            MovieId = movieId,
            ShowtimeId = showtimeId,
            Seats = seats,
            SubtotalAmount = subtotal,
            PromoCode = promoCode,
            DiscountAmount = discountAmount,
            TotalAmount = subtotal - discountAmount,
            BookedAt = DateTime.Now
        };
        Bookings[booking.Reference] = booking;

        // Mark the seats as taken so nobody else can grab them in-session.
        var taken = BookedSeats.GetOrAdd(showtimeId, _ => new HashSet<string>());
        lock (taken)
        {
            foreach (var s in seats) taken.Add(s);
        }
        return booking;
    }

    public static Booking? GetBooking(string reference) =>
        Bookings.TryGetValue(reference, out var booking) ? booking : null;

    /// <summary>All bookings made with a given mobile number (digits compared).</summary>
    public static List<Booking> GetBookingsByPhone(string phone)
    {
        var digits = DigitsOnly(phone);
        if (string.IsNullOrEmpty(digits)) return new();
        return Bookings.Values
            .Where(b => DigitsOnly(b.Phone) == digits)
            .OrderByDescending(b => b.BookedAt)
            .ToList();
    }

    /// <summary>
    /// Cancels a booking and frees its seats so they become bookable again.
    /// Returns false when the reference doesn't exist.
    /// </summary>
    public static bool CancelBooking(string reference)
    {
        if (!Bookings.TryRemove(reference, out var booking)) return false;
        if (BookedSeats.TryGetValue(booking.ShowtimeId, out var taken))
        {
            lock (taken)
            {
                foreach (var s in booking.Seats) taken.Remove(s);
            }
        }
        return true;
    }
}
