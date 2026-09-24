using System.Text;
using System.Text.Json;

namespace CineMagic.Services;

/// <summary>
/// Real SMS sender via the MSG91 Flow API (https://control.msg91.com/api/v5/flow/).
/// Requires YOUR OWN free MSG91 account: create an approved SMS flow (template) in
/// the MSG91 dashboard, then set "Sms:ApiKey", "Sms:SenderId" and "Sms:FlowId" in
/// appsettings.json and register this class in Program.cs instead of NullSmsService.
/// See README.md for the full setup steps.
/// </summary>
public class Msg91SmsService : ISmsService
{
    private readonly HttpClient _http;
    private readonly ILogger<Msg91SmsService> _logger;
    private readonly string _apiKey;
    private readonly string _senderId;
    private readonly string _flowId;

    public Msg91SmsService(HttpClient http, IConfiguration config, ILogger<Msg91SmsService> logger)
    {
        _http = http;
        _logger = logger;
        _apiKey = config["Sms:ApiKey"] ?? string.Empty;
        _senderId = config["Sms:SenderId"] ?? "CINEMG";
        _flowId = config["Sms:FlowId"] ?? string.Empty;
    }

    public async Task SendBookingSmsAsync(string phone, string message)
    {
        if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_flowId))
        {
            _logger.LogWarning("[SMS:msg91] Skipped — Sms:ApiKey or Sms:FlowId is not configured.");
            return;
        }

        // MSG91 expects the 10-digit Indian mobile number (optionally with 91 prefix).
        var mobile = phone.StartsWith("91") ? phone : "91" + phone;

        var payload = new
        {
            flow_id = _flowId,
            sender = _senderId,
            mobiles = mobile,
            message
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://control.msg91.com/api/v5/flow/");
        request.Headers.Add("authkey", _apiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var response = await _http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            _logger.LogInformation("[SMS:msg91] Sent to {Phone}. Response: {Body}", phone, body);
        else
            _logger.LogError("[SMS:msg91] Failed for {Phone}. Status: {Status}. Body: {Body}",
                phone, response.StatusCode, body);
    }
}
