# 🚀 CineMagic Deployment Guide

## Goal

Deploy CineMagic so that the GitHub repository contains the source code and a public URL opens the running ASP.NET Core application in any browser.

## Recommended Architecture

```text
Developer PC
    │
    ├── Git
    ▼
GitHub repository
    │
    │ push main
    ▼
GitHub Actions
    │
    ├── dotnet restore
    ├── dotnet build
    ├── dotnet test
    └── dotnet publish
    │
    ▼
Azure App Service
    │
    ▼
Public HTTPS URL
```

## 1. Prepare the Repository

Before the first push:

```bash
dotnet restore
dotnet build --configuration Release
dotnet run
```

Test the complete booking flow locally.

## 2. Protect Secrets Before Git

The project must not contain real SMS API keys.

If a secret has already been committed, simply deleting it from the current file is not enough. Rotate/revoke the exposed key at the provider first, then remove it from Git history if necessary.

Recommended local approach:

```bash
dotnet user-secrets init
dotnet user-secrets set "Sms:Fast2SmsKey" "YOUR_KEY"
```

For production, use Azure App Service Application Settings.

## 3. Create the GitHub Repository

Recommended repository name:

```text
CineMagic
```

Suggested description:

```text
A cinematic ASP.NET Core MVC movie ticket booking system with interactive seat selection, animated UI, e-tickets and invoice generation.
```

Suggested topics:

```text
aspnet-core
aspnet-mvc
csharp
dotnet
movie-booking
cinema
razor
web-development
full-stack
college-project
```

Keep the repository **Public** if you want it to be a portfolio project.

## 4. First Push

If Visual Studio's "Create a Git repository" dialog is open, you can use it.

Or from the project directory:

```bash
git init -b main
git add .
git commit -m "feat: initial CineMagic movie booking system"
git remote add origin https://github.com/dipak90000/CineMagic.git
git push -u origin main
```

Then verify that sensitive files are not tracked:

```bash
git status
git ls-files
```

## 5. Create Azure App Service

Create an Azure App Service for the ASP.NET Core application.

Use:

- Runtime: the .NET version targeted by `CineMagic.csproj`
- Operating system: Windows or Linux
- Region: a region appropriate for your users
- Deployment: GitHub Actions

Azure App Service provides a public web endpoint.

## 6. Configure Production Settings

Do not put API keys into `appsettings.json`.

In Azure App Service:

**Settings → Environment variables → App settings**

Add:

```text
Sms__Fast2SmsKey = YOUR_PRODUCTION_KEY
```

Or configure the SMS provider you actually use.

## 7. GitHub Actions

The workflow should run whenever `main` changes.

Recommended pipeline:

```text
push main
   ↓
checkout
   ↓
setup .NET
   ↓
restore
   ↓
build Release
   ↓
test
   ↓
publish
   ↓
deploy Azure App Service
```

The exact Azure credentials and app name should be stored as GitHub/Azure secrets rather than committed to the repository.

## 8. Verify Deployment

After deployment:

1. Open the Azure App Service URL.
2. Test Home.
3. Test Movies.
4. Test seat selection.
5. Complete a demo booking.
6. Open the invoice.
7. Test My Bookings.
8. Test cancellation.
9. Test on mobile width.
10. Confirm HTTPS works.

## 9. Add the Live Demo to README

Replace:

```text
YOUR_LIVE_SITE_URL
```

with your real Azure URL.

Example:

```markdown
[🌐 Live Demo](https://cinemagic-example.azurewebsites.net)
```

## 10. Continuous Deployment

Once GitHub Actions is configured, future pushes to `main` can automatically build and deploy the application.

Typical workflow:

```bash
git add .
git commit -m "feat: improve seat selection UX"
git push origin main
```

GitHub Actions then builds and deploys the new version.

## 11. Academic Demo Checklist

Before showing the project to your professor:

### UI
- [ ] Desktop layout
- [ ] Mobile layout
- [ ] Animations
- [ ] Loading states
- [ ] Empty states
- [ ] Error states

### Functionality
- [ ] Movie search
- [ ] Filters
- [ ] Showtimes
- [ ] Seat selection
- [ ] Price calculation
- [ ] Promo code
- [ ] Booking
- [ ] Confirmation
- [ ] Invoice
- [ ] Cancellation

### Engineering
- [ ] No API keys in Git
- [ ] Release build succeeds
- [ ] Deployment succeeds
- [ ] Public URL works
- [ ] README is complete
- [ ] Screenshots added
- [ ] Architecture documented

## Troubleshooting

### App works locally but not on Azure

Check:

- Target .NET runtime
- Azure App Service runtime
- Application logs
- Environment variables
- HTTPS configuration
- File paths
- External API configuration

### Deployment succeeds but the site shows an error

Open Azure App Service:

**Monitoring → Log stream**

Check the startup exception.

### SMS does not work

The application is intentionally designed to keep booking functional even when SMS is unavailable. Verify the production environment variable and provider account separately.
