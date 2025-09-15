namespace Dealership.Api.Models;

public class User
{
    // Unique identifier for each user
    public Guid Id { get; set; }

    // Email used as username (must be unique)
    public string Email { get; set; } = default!;

    // Password hash (never store plain text)
    public string PasswordHash { get; set; } = default!;

    // Role can be "Admin" or "Customer"
    public string Role { get; set; } = "Customer";

    // Date when the account was created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
