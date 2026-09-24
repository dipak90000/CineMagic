using System.ComponentModel.DataAnnotations;

namespace CineMagic.Models;

/// <summary>View-model for the contact page enquiry form.</summary>
public class ContactViewModel
{
    [Required(ErrorMessage = "Please tell us your name.")]
    [StringLength(60, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "We need your email to reply.")]
    [EmailAddress(ErrorMessage = "That doesn't look like a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please choose a topic.")]
    public string Topic { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please write your message.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Message must be at least 10 characters.")]
    public string Message { get; set; } = string.Empty;
}
