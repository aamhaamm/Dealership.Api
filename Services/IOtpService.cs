using Dealership.Api.Models;

namespace Dealership.Api.Services;

// OTP Service contract
public interface IOtpService
{
    Task<OtpToken> GenerateAsync(string purpose, string identifier, TimeSpan ttl);
    Task<bool> ValidateAsync(Guid otpId, string code, string purpose, string identifier, int maxAttempts = 5);
}
