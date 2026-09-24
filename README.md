# 🎬 CineMagic — Cinematic Movie Ticket Booking System

<p align="center">
  <strong>A premium ASP.NET Core MVC movie-booking experience with cinematic UI, interactive seat selection, booking flow, e-tickets and invoice generation.</strong>
</p>

<p align="center">
  <a href="YOUR_LIVE_SITE_URL">🌐 Live Demo</a> ·
  <a href="https://github.com/dipak90000/CineMagic">📦 Source Code</a> ·
  <a href="docs/DEPLOYMENT.md">🚀 Deployment</a>
</p>

---

## ✨ Overview

**CineMagic** is a college full-stack web project designed as a modern cinema-booking platform rather than a basic CRUD application.

The current implementation uses **ASP.NET Core 8 MVC**, Razor Views, C#, CSS and vanilla JavaScript. It provides a complete customer journey:

> **Discover → Select Movie → Choose Showtime → Select Seats → Checkout → Confirmation → Invoice**

The project currently uses an **in-memory data service**, so it can run without a database. This makes the demo portable and easy to deploy; the data layer can later be replaced with Entity Framework Core + SQL Server.

## 🎯 Project Goals

- Deliver a polished, cinematic user experience.
- Demonstrate ASP.NET Core MVC architecture.
- Implement server-side validation and booking logic.
- Provide interactive seat selection and dynamic pricing.
- Demonstrate cancellation, booking lookup and invoice generation.
- Keep external integrations optional so the core booking flow remains reliable.

## 🚀 Key Features

### 🎞️ Movie Discovery
- Cinematic animated hero section
- Now Showing movie grid
- Movie search
- Category filtering
- Rating and metadata badges
- Responsive movie cards
- Hover/3D tilt interactions

### 🎟️ Booking
- Movie details and showtime selection
- Multi-day showtime picker
- Interactive A–H seat map
- Available / selected / sold seat states
- Three seat price tiers
- Live total calculation
- Maximum seat selection limit
- Promo-code handling

### 💳 Checkout
- Floating-label form
- Server-side validation
- Animated Pay & Book interaction
- Discount breakdown
- GST calculation
- Booking reference generation

### 🎉 Confirmation & Ticket
- Animated confirmation screen
- E-ticket layout
- Booking reference
- Ticket details
- Confetti animation
- Invoice generation

### 🧾 Invoice
- Customer and show information
- Seat-wise pricing
- Subtotal
- Discount
- GST
- Grand total
- Print / Save as PDF

### 📱 Booking Management
- Find bookings by mobile number
- View previous tickets
- Cancel bookings
- Release cancelled seats back to availability

### 📩 SMS Integration
The project supports an abstraction-based SMS layer:
- Safe demo/mock sender
- Fast2SMS integration
- MSG91 integration

**Never commit real API keys to GitHub.**

## 🧱 Architecture

```text
┌───────────────────────────────────────────────┐
│                 Browser / UI                  │
│  Razor Views + CSS + Vanilla JavaScript      │
│  Animations · Seat Map · Responsive Design   │
└───────────────────────┬───────────────────────┘
                        │ HTTP
┌───────────────────────▼───────────────────────┐
│              ASP.NET Core MVC                 │
│                                               │
│ Controllers → Services → Models/ViewModels    │
│ Validation · Booking Rules · SMS Abstraction  │
└───────────────────────┬───────────────────────┘
                        │
┌───────────────────────▼───────────────────────┐
│             Data / Integration Layer          │
│                                               │
│ In-Memory DataService                         │
│ Optional SMS Providers                        │
│ Future: EF Core + SQL Server                  │
└───────────────────────────────────────────────┘
```

## 🗂️ Project Structure

```text
CineMagic/
├── Controllers/
│   ├── HomeController.cs
│   └── BookingController.cs
│
├── Models/
│   ├── Movie.cs
│   ├── Showtime.cs
│   ├── Booking.cs
│   ├── SeatMapViewModel.cs
│   ├── CheckoutViewModel.cs
│   ├── ConfirmationViewModel.cs
│   ├── InvoiceViewModel.cs
│   ├── MyBookingsViewModel.cs
│   └── ContactViewModel.cs
│
├── Services/
│   ├── DataService.cs
│   ├── ISmsService.cs
│   ├── NullSmsService.cs
│   ├── Fast2SmsService.cs
│   └── Msg91SmsService.cs
│
├── Views/
│   ├── Home/
│   ├── Booking/
│   └── Shared/
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── images/
│
├── .github/
│   └── workflows/
├── docs/
├── Program.cs
├── CineMagic.csproj
├── appsettings.json
├── .gitignore
└── README.md
```

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| Pattern | MVC |
| Language | C# 12 |
| UI | Razor Views |
| Styling | CSS |
| Interactivity | Vanilla JavaScript |
| Data | In-memory service |
| Optional database | SQL Server + EF Core |
| Optional SMS | Fast2SMS / MSG91 |
| Version control | Git + GitHub |
| Hosting | Azure App Service |

## 💻 Run Locally

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022/2026 or VS Code + C# Dev Kit
- Git

### Start

```bash
git clone https://github.com/dipak90000/CineMagic.git
cd CineMagic
dotnet restore
dotnet run
```

Open the HTTPS URL printed by ASP.NET Core.

### Visual Studio

1. Open `CineMagic.sln` / project.
2. Select the HTTPS launch profile.
3. Press **F5**.
4. The browser will open automatically.

## 🔐 Configuration & Secrets

Do **not** store production API keys in source control.

Use one of these approaches:

### Local development — User Secrets

```bash
dotnet user-secrets init
dotnet user-secrets set "Sms:Fast2SmsKey" "YOUR_KEY"
```

### Production — Environment Variables / Azure App Settings

Configure:

```text
Sms__Fast2SmsKey
Sms__ApiKey
Sms__FlowId
Sms__SenderId
```

The double underscore (`__`) maps to nested ASP.NET Core configuration keys.

## 🧪 Testing Checklist

Before pushing a release:

- [ ] Home page loads
- [ ] Movie search works
- [ ] Category filters work
- [ ] Movie details open
- [ ] Showtime selection works
- [ ] Seat selection updates price
- [ ] Sold seats cannot be selected
- [ ] Checkout validation works
- [ ] Invalid promo code does not break booking
- [ ] Booking confirmation is generated
- [ ] Invoice opens and prints
- [ ] My Bookings lookup works
- [ ] Cancellation releases seats
- [ ] App works without SMS credentials
- [ ] No secrets are present in Git history

## 🚀 Deployment

The recommended hosting architecture is:

```text
GitHub
   │
   │ push to main
   ▼
GitHub Actions
   │
   ├── Restore
   ├── Build
   ├── Test
   └── Publish
        │
        ▼
Azure App Service
        │
        ▼
https://<your-app>.azurewebsites.net
```

See **[Deployment Guide](docs/DEPLOYMENT.md)**.

After deployment, put the public URL at the top of this README:

```text
https://YOUR-APP.azurewebsites.net
```

That gives anyone with the link a direct browser-accessible demo.

## 📌 Important: GitHub Pages

GitHub Pages is designed for static sites and is **not the correct host for the ASP.NET Core server application**.

For CineMagic, use a server host such as **Azure App Service**.

## 🧭 Booking Flow

```text
Home
  ↓
Movies
  ↓
Movie Details
  ↓
Showtime
  ↓
Seat Selection
  ↓
Checkout
  ↓
Confirmation / E-Ticket
  ↓
Invoice
```

## 🏆 Suggested Future Enhancements

For a stronger production-style version:

- Entity Framework Core + SQL Server
- ASP.NET Core Identity
- Admin dashboard
- Real authentication
- Real payment gateway
- Persistent booking database
- Cinema/screen management
- Email notifications
- QR-code tickets
- Real-time seat locking
- CI/CD deployment
- Automated unit/integration tests
- Application logging and monitoring

## 👨‍💻 Author

**Dipak Bariya**

GitHub: [@dipak90000](https://github.com/dipak90000)

---

## 📄 License

For academic/portfolio use. Add an OSI-approved open-source license if you intend to distribute the project as open source.
