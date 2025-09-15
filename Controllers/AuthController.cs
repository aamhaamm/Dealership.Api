using Dealership.Api.Data;
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

    public AuthController(AppDbContext db, IJwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    // Register new customer
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User request)
    {
        // Check if email already exists
        if (await _db.Users.AnyAsync(u => u.Email == request.Email.ToLower()))
            return Conflict(new { message = "Email already registered" });

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash),
            Role = "Customer"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwt.CreateToken(user);
        return Ok(new { token, role = user.Role, email = user.Email });
    }

    // Login existing user
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.PasswordHash, user.PasswordHash))
            return Unauthorized(new { message = "Invalid credentials" });

        var token = _jwt.CreateToken(user);
        return Ok(new { token, role = user.Role, email = user.Email });
    }
}
