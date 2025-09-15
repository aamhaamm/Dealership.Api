namespace Dealership.Api.Models;

public class OtpToken
{
    public Guid Id { get; set; }

    // Purpose can be: login, register, purchase, updateVehicle
    public string Purpose { get; set; } = default!;

    // Identifier is usually the email (or phone if extended)
    public string Identifier { get; set; } = default!;

    // Store OTP as hash (never plain text)
    public string OtpHash { get; set; } = default!;

    public DateTime ExpiresAtUtc { get; set; }
    public int Attempts { get; set; } = 0;
    public bool Used { get; set; } = false;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
