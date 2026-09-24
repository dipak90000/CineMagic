namespace CineMagic.Services;

/// <summary>
/// Abstraction over SMS sending. The default registration is
/// <see cref="NullSmsService"/> (logs only, never sends), so bookings can
/// never break because of SMS. Swap the DI registration in Program.cs to
/// <c>Msg91SmsService</c> once you have your own MSG91 gateway account.
/// </summary>
public interface ISmsService
{
    /// <summary>Sends a booking confirmation text to the given 10-digit mobile number.</summary>
    Task SendBookingSmsAsync(string phone, string message);
}
