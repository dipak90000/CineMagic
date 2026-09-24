namespace CineMagic.Services;

/// <summary>
/// No-op SMS implementation: writes the message to the log instead of
/// sending it. This is the default registration — safe for demos and local
/// development, and it can never fail a booking.
/// </summary>
public class NullSmsService : ISmsService
{
    private readonly ILogger<NullSmsService> _logger;

    public NullSmsService(ILogger<NullSmsService> logger)
    {
        _logger = logger;
    }

    public Task SendBookingSmsAsync(string phone, string message)
    {
        _logger.LogInformation("[SMS:mock] To {Phone}: {Message}", phone, message);
        return Task.CompletedTask;
    }
}
