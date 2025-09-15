using Dealership.Api.Data;
using Dealership.Api.Dtos;
using Dealership.Api.Models;
using Dealership.Api.Services;
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

    public AuthController(AppDbContext db, IJwtService jwt, IOtpService otp)
    {
        _db = db;
        _jwt = jwt;
        _otp = otp;
    }

    // Step 1: Request OTP (for register or login)
    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] OtpRequest req)
    {
        var token = await _otp.GenerateAsync(req.Purpose, req.Identifier, TimeSpan.FromMinutes(5));
        return Ok(new { OtpId = token.Id, ExpiresAtUtc = token.ExpiresAtUtc });
    }

    // Step 2: Register new customer (OTP required)
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterWithOtpRequest req)
    {
        var valid = await _otp.ValidateAsync(Guid.Parse(req.OtpId), req.Code, "register", req.Email);
        if (!valid) return BadRequest(new { message = "Invalid or expired OTP" });

        if (await _db.Users.AnyAsync(u => u.Email == req.Email.ToLower()))
            return Conflict(new { message = "Email already registered" });

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = req.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = "Customer"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwt.CreateToken(user);
        return new AuthResponse(token, user.Role, user.Email);
    }

    // Step 3: Login existing user (OTP required)
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginWithOtpRequest req)
    {
        var valid = await _otp.ValidateAsync(Guid.Parse(req.OtpId), req.Code, "login", req.Email);
        if (!valid) return BadRequest(new { message = "Invalid or expired OTP" });

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email.ToLower());
        if (user is null) return Unauthorized(new { message = "User not found" });

        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid credentials" });

        var token = _jwt.CreateToken(user);
        return new AuthResponse(token, user.Role, user.Email);
    }
}
