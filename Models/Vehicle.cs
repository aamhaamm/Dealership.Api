namespace Dealership.Api.Models;

public class Vehicle
{
    public Guid Id { get; set; }

    // Basic details
    public string Make { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }

    // Pricing and details
    public decimal Price { get; set; }
    public string Color { get; set; } = "White";
    public int MileageKm { get; set; }

    // Availability status
    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
