using Dealership.Api.Data;
using Dealership.Api.Dtos;
using Dealership.Api.Models;
using Dealership.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dealership.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "CustomerOnly")]
public class PurchasesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IOtpService _otp;

    public PurchasesController(AppDbContext db, IOtpService otp)
    {
        _db = db;
        _otp = otp;
    }

    // Customer requests a purchase (OTP required)
    [HttpPost("request")]
    public async Task<IActionResult> RequestPurchase([FromBody] PurchaseRequest req)
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null) return Unauthorized();

        // Validate OTP
        var valid = await _otp.ValidateAsync(Guid.Parse(req.OtpId), req.Code, "purchase", email);
        if (!valid) return BadRequest(new { message = "Invalid or expired OTP" });

        var vehicle = await _db.Vehicles.FindAsync(req.VehicleId);
        if (vehicle is null || !vehicle.IsAvailable)
            return BadRequest(new { message = "Vehicle not available" });

        // Create purchase
        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicle.Id,
            UserId = user.Id,            
            SalePrice = vehicle.Price,
            PurchasedAt = DateTime.UtcNow
        };

        vehicle.IsAvailable = false; // Mark as sold
        _db.Purchases.Add(purchase);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Purchase completed successfully", purchase.Id });
    }

    // Customer views their purchase history
    [HttpGet("history")]
    public async Task<IEnumerable<PurchaseResponse>> History()
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null) return Enumerable.Empty<PurchaseResponse>();

        var purchases = await _db.Purchases
            .Where(p => p.UserId == user.Id)  
            .OrderByDescending(p => p.PurchasedAt)
            .ToListAsync();

        return purchases.Select(p => new PurchaseResponse(p.Id, p.VehicleId, p.SalePrice, p.PurchasedAt));
    }
}
