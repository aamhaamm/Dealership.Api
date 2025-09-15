using System.ComponentModel.DataAnnotations;

namespace Dealership.Api.Dtos;

// OTP
public record OtpRequest(
    [Required] string Purpose,
    [Required, EmailAddress] string Identifier
);

public record OtpVerify(
    [Required] string OtpId,
    [Required] string Code
);

// Register with OTP
public record RegisterWithOtpRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required] string OtpId,
    [Required] string Code
);

// Login with OTP
public record LoginWithOtpRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required] string OtpId,
    [Required] string Code
);

// Auth response
public record AuthResponse(string Token, string Role, string Email);
