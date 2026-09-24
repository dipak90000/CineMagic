namespace CineMagic.Services;

/// <summary>
/// Real SMS sender via Fast2SMS (https://www.fast2sms.com) — the simplest
/// Indian gateway: no DLT registration needed on the quick route.
/// Reads the API key from configuration "Sms:Fast2SmsKey".
///
/// Setup (5 minutes, free): register at fast2sms.com with your mobile
/// number, verify the OTP, open "Dev API", copy the API key, paste it into
/// appsettings.json → "Sms:Fast2SmsKey", restart the app. Every booking SMS
/// will then arrive on the phone number typed at checkout.
///
/// Failures throw InvalidOperationException so the caller can log them —
/// BookingController wraps every send in try/catch, so SMS can never break
/// a booking.
/// </summary>
public class Fast2SmsService : ISmsService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<Fast2SmsService> _logger;

    public Fast2SmsService(HttpClient http, IConfiguration config, ILogger<Fast2SmsService> logger)
    {
        _http = http;
        _config = config;
        _logger = logger;
    }

    public async Task SendBookingSmsAsync(string phone, string message)
    {
        var key = _config["Sms:Fast2SmsKey"];
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Fast2SMS API key is not configured (Sms:Fast2SmsKey).");

        // Fast2SMS expects the 10-digit Indian mobile number.
        var digits = DataService.DigitsOnly(phone);
        if (digits.Length == 12 && digits.StartsWith("91")) digits = digits.Substring(2);
        if (digits.Length == 11 && digits.StartsWith("0")) digits = digits.Substring(1);
        if (digits.Length != 10)
            throw new InvalidOperationException($"Fast2SMS needs a 10-digit mobile number, got '{phone}'.");

        var url = "https://www.fast2sms.com/dev/bulkV2"
            + "?authorization=" + Uri.EscapeDataString(key)
            + "&message=" + Uri.EscapeDataString(message)
            + "&language=english&route=q&numbers=" + digits;

        using var response = await _http.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();

        // Fast2SMS returns JSON like {"return":true,...}; any false means rejected.
        var compact = body.Replace(" ", string.Empty).Replace("\t", string.Empty);
        var failed = !response.IsSuccessStatusCode || compact.Contains("\"return\":false");

        if (failed)
            throw new InvalidOperationException(
                $"Fast2SMS send failed (HTTP {(int)response.StatusCode}). Response: {body}");

        _logger.LogInformation("[SMS:fast2sms] Sent to {Phone}. Response: {Body}", digits, body);
    }
}
