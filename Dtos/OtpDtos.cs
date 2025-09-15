namespace Dealership.Api.Dtos;

// Request OTP
public record OtpRequest(string Purpose, string Identifier);

// Verify OTP
public record OtpVerify(string OtpId, string Code);
