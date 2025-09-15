using Dealership.Api.Data;
using Dealership.Api.Dtos;
using Dealership.Api.Models;
using Dealership.Api.Services;
using Dealership.Api.Common;
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
    private readonly ILogger<PurchasesController> _logger;

    public PurchasesController(AppDbContext db, IOtpService otp, ILogger<PurchasesController> logger)
    {
        _db = db;
        _otp = otp;
        _logger = logger;
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestPurchase([FromBody] PurchaseRequest req)
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null) return Unauthorized(new { code = "USER_NOT_FOUND", message = "User not found" });

        var valid = await _otp.ValidateAsync(Guid.Parse(req.OtpId), req.Code, OtpPurposes.Purchase, email);
        if (!valid) return BadRequest(new { code = "OTP_INVALID", message = "Invalid or expired OTP" });

        var vehicle = await _db.Vehicles.FindAsync(req.VehicleId);
        if (vehicle is null || !vehicle.IsAvailable)
            return BadRequest(new { code = "VEHICLE_UNAVAILABLE", message = "Vehicle not available" });

        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicle.Id,
            UserId = user.Id,
            SalePrice = vehicle.Price,
            PurchasedAt = DateTime.UtcNow,
            Status = "Pending"
        };

        vehicle.IsAvailable = false;
        _db.Purchases.Add(purchase);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Purchase requested: {PurchaseId} by {Email}", purchase.Id, email);

        return Ok(new { message = "Purchase request submitted successfully", purchase.Id });
    }

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
