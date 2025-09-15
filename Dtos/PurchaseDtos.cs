namespace Dealership.Api.Dtos;

// For creating a purchase request
public record PurchaseRequest(Guid VehicleId, string OtpId, string Code);

// For returning purchase history
public record PurchaseResponse(Guid Id, Guid VehicleId, decimal SalePrice, DateTime PurchasedAt);
