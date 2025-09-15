namespace Dealership.Api.Dtos;

// For browsing vehicles
public record VehicleResponse(
    Guid Id,
    string Make,
    string Model,
    int Year,
    decimal Price,
    string Color,
    int MileageKm,
    bool IsAvailable
);

// For adding a new vehicle
public record VehicleCreateRequest(
    string Make,
    string Model,
    int Year,
    decimal Price,
    string Color,
    int MileageKm
);

// For updating vehicle (OTP protected)
public record VehicleUpdateRequest(
    Guid Id,
    string Make,
    string Model,
    int Year,
    decimal Price,
    string Color,
    int MileageKm,
    string OtpId,
    string Code
);
