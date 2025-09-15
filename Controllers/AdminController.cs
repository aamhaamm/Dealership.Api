using Dealership.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dealership.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<AdminController> _logger;

    public AdminController(AppDbContext db, ILogger<AdminController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetAllCustomers()
    {
        var customers = await _db.Users
            .Where(u => u.Role == "Customer")
            .Select(u => new
            {
                u.Id,
                u.Email,
                u.CreatedAt
            })
            .ToListAsync();

        return Ok(customers);
    }

    [HttpPost("process-sale/{purchaseId}")]
    public async Task<IActionResult> ProcessSale(Guid purchaseId, [FromQuery] bool approve)
    {
        var purchase = await _db.Purchases.FirstOrDefaultAsync(p => p.Id == purchaseId);
        if (purchase == null)
            return NotFound(new { code = "PURCHASE_NOT_FOUND", message = "Purchase not found" });

        if (purchase.Status != "Pending")
            return BadRequest(new { code = "ALREADY_PROCESSED", message = "Purchase already processed" });

        purchase.Status = approve ? "Approved" : "Declined";
        await _db.SaveChangesAsync();

        _logger.LogInformation("Purchase {PurchaseId} processed: {Status}", purchase.Id, purchase.Status);

        return Ok(new
        {
            purchase.Id,
            purchase.UserId,
            purchase.VehicleId,
            purchase.Status
        });
    }
}
