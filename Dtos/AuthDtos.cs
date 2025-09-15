namespace Dealership.Api.Dtos;

// OTP
public record OtpRequest(string Purpose, string Identifier);
public record OtpVerify(string OtpId, string Code);

// Register with OTP
public record RegisterWithOtpRequest(string Email, string Password, string OtpId, string Code);

// Login with OTP
public record LoginWithOtpRequest(string Email, string Password, string OtpId, string Code);

// Auth response
public record AuthResponse(string Token, string Role, string Email);
