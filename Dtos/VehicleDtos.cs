using System.ComponentModel.DataAnnotations;

namespace Dealership.Api.Dtos;

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

public record VehicleCreateRequest(
    [Required] string Make,
    [Required] string Model,
    [Range(1900, 2100)] int Year,
    [Range(1, double.MaxValue)] decimal Price,
    [Required] string Color,
    [Range(0, int.MaxValue)] int MileageKm
);

public record VehicleUpdateRequest(
    [Required] Guid Id,
    [Required] string Make,
    [Required] string Model,
    [Range(1900, 2100)] int Year,
    [Range(1, double.MaxValue)] decimal Price,
    [Required] string Color,
    [Range(0, int.MaxValue)] int MileageKm,
    [Required] string OtpId,
    [Required] string Code
);
