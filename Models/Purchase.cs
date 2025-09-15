namespace Dealership.Api.Models;

public class Purchase
{
    public Guid Id { get; set; }

    // References
    public Guid VehicleId { get; set; }
    public Guid UserId { get; set; }

    // Sale details
    public decimal SalePrice { get; set; }
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending"; 
}
