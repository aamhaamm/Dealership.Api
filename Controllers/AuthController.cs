using Dealership.Api.Data;
using Dealership.Api.Dtos;
using Dealership.Api.Models;
using Dealership.Api.Services;
using Dealership.Api.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dealership.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJwtService _jwt;
    private readonly IOtpService _otp;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AppDbContext db, IJwtService jwt, IOtpService otp, ILogger<AuthController> logger)
    {
        _db = db;
        _jwt = jwt;
        _otp = otp;
        _logger = logger;
    }

    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] OtpRequest req)
    {
        var token = await _otp.GenerateAsync(req.Purpose, req.Identifier, TimeSpan.FromMinutes(5));
        _logger.LogInformation("OTP generated for {Purpose} - {Identifier}", req.Purpose, req.Identifier);
        return Ok(new { OtpId = token.Id, ExpiresAtUtc = token.ExpiresAtUtc });
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterWithOtpRequest req)
    {
        var valid = await _otp.ValidateAsync(Guid.Parse(req.OtpId), req.Code, OtpPurposes.Register, req.Email);
        if (!valid) return BadRequest(new { code = "OTP_INVALID", message = "Invalid or expired OTP" });

        if (await _db.Users.AnyAsync(u => u.Email == req.Email.ToLower()))
            return Conflict(new { code = "EMAIL_EXISTS", message = "Email already registered" });

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = req.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = "Customer"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        _logger.LogInformation("New customer registered: {Email}", user.Email);

        var token = _jwt.CreateToken(user);
        return new AuthResponse(token, user.Role, user.Email);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginWithOtpRequest req)
    {
        var valid = await _otp.ValidateAsync(Guid.Parse(req.OtpId), req.Code, OtpPurposes.Login, req.Email);
        if (!valid) return BadRequest(new { code = "OTP_INVALID", message = "Invalid or expired OTP" });

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email.ToLower());
        if (user is null) return Unauthorized(new { code = "USER_NOT_FOUND", message = "User not found" });

        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized(new { code = "INVALID_CREDENTIALS", message = "Invalid credentials" });

        _logger.LogInformation("User logged in: {Email}", user.Email);

        var token = _jwt.CreateToken(user);
        return new AuthResponse(token, user.Role, user.Email);
    }
}
