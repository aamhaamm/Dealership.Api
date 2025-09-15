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

    // Password must be strong (min 8 chars, upper, lower, number, special char)
    [Required, MinLength(8), RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
        ErrorMessage = "Password must contain uppercase, lowercase, number, and special character.")]
    string Password,

    [Required] string OtpId,
    [Required] string Code
);

// Login with OTP
public record LoginWithOtpRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [Required] string OtpId,
    [Required] string Code
);

// Auth response
public record AuthResponse(string Token, string Role, string Email);
