// CineMagic - entry point.
// Standard ASP.NET Core minimal hosting setup with MVC + static files.
using CineMagic.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services (controllers + Razor views).
builder.Services.AddControllersWithViews();

// SMS: auto-select the provider based on configuration.
//   1. "Sms:Fast2SmsKey" present -> Fast2SmsService (real SMS, simplest Indian gateway)
//   2. "Sms:ApiKey" present       -> Msg91SmsService (alternative gateway)
//   3. otherwise                  -> NullSmsService (safe demo mode: logs only)
// BookingController wraps every send in try/catch, so SMS can NEVER break a booking.
// See README "SMS notifications" for the 5-minute Fast2SMS setup.
var fast2SmsKey = builder.Configuration["Sms:Fast2SmsKey"];
var msg91Key = builder.Configuration["Sms:ApiKey"];
if (!string.IsNullOrWhiteSpace(fast2SmsKey))
    builder.Services.AddHttpClient<ISmsService, Fast2SmsService>();
else if (!string.IsNullOrWhiteSpace(msg91Key))
    builder.Services.AddHttpClient<ISmsService, Msg91SmsService>();
else
    builder.Services.AddScoped<ISmsService, NullSmsService>();

var app = builder.Build();

// Announce the active SMS mode at startup so it's visible in the
// Visual Studio Output window — makes "is SMS working?" trivial to answer.
var smsMode = !string.IsNullOrWhiteSpace(fast2SmsKey) ? "REAL SMS via Fast2SMS"
    : !string.IsNullOrWhiteSpace(msg91Key) ? "REAL SMS via MSG91"
    : "DEMO MODE — no real SMS will be sent (paste your key into Sms:Fast2SmsKey in appsettings.json)";
app.Logger.LogWarning("CineMagic SMS mode: {Mode}", smsMode);

// Standard middleware pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();          // serves wwwroot (css, js)

app.UseRouting();
app.UseAuthorization();

// Conventional route: /{controller}/{action}/{id?}
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
