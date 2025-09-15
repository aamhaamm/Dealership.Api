using Dealership.Api.Data;
using Dealership.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Dealership.Api.Services;

public class OtpService : IOtpService
{
    private readonly AppDbContext _db;

    public OtpService(AppDbContext db)
    {
        _db = db;
    }

    // Generate OTP and save to DB
    public async Task<OtpToken> GenerateAsync(string purpose, string identifier, TimeSpan ttl)
    {
        var code = Random.Shared.Next(100000, 999999).ToString(); // 6-digit code

        var token = new OtpToken
        {
            Id = Guid.NewGuid(),
            Purpose = purpose,
            Identifier = identifier.ToLower(),
            OtpHash = BCrypt.Net.BCrypt.HashPassword(code),
            ExpiresAtUtc = DateTime.UtcNow.Add(ttl),
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.OtpTokens.Add(token);
        await _db.SaveChangesAsync();

        // Simulate sending OTP (console output)
        Console.WriteLine($"[OTP:{purpose}] {identifier} => {code} (ID: {token.Id})");

        return token;
    }

    // Validate OTP
    public async Task<bool> ValidateAsync(Guid otpId, string code, string purpose, string identifier, int maxAttempts = 5)
    {
        var token = await _db.OtpTokens.FirstOrDefaultAsync(o => o.Id == otpId);
        if (token is null) return false;
        if (token.Used) return false;
        if (token.Purpose != purpose) return false;
        if (token.Identifier != identifier.ToLower()) return false;
        if (DateTime.UtcNow > token.ExpiresAtUtc) return false;
        if (token.Attempts >= maxAttempts) return false;

        token.Attempts++;
        var ok = BCrypt.Net.BCrypt.Verify(code, token.OtpHash);

        if (ok) token.Used = true;

        await _db.SaveChangesAsync();
        return ok;
    }
}
