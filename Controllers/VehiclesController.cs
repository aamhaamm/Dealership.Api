using Dealership.Api.Data;
using Dealership.Api.Dtos;
using Dealership.Api.Models;
using Dealership.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dealership.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IOtpService _otp;

    public VehiclesController(AppDbContext db, IOtpService otp)
    {
        _db = db;
        _otp = otp;
    }

    // Browse vehicles (any customer)
    [HttpGet("browse")]
    [Authorize(Policy = "CustomerOnly")]
    public async Task<IEnumerable<VehicleResponse>> Browse()
    {
        var vehicles = await _db.Vehicles.Where(v => v.IsAvailable).ToListAsync();
        return vehicles.Select(v => new VehicleResponse(v.Id, v.Make, v.Model, v.Year, v.Price, v.Color, v.MileageKm, v.IsAvailable));
    }

    // Get vehicle details
    [HttpGet("{id}")]
    [Authorize] // Any logged-in user
    public async Task<ActionResult<VehicleResponse>> GetById(Guid id)
    {
        var v = await _db.Vehicles.FindAsync(id);
        if (v is null) return NotFound();

        return new VehicleResponse(v.Id, v.Make, v.Model, v.Year, v.Price, v.Color, v.MileageKm, v.IsAvailable);
    }

    // Add vehicle (Admin only)
    [HttpPost("add")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<VehicleResponse>> Add([FromBody] VehicleCreateRequest req)
    {
        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            Make = req.Make,
            Model = req.Model,
            Year = req.Year,
            Price = req.Price,
            Color = req.Color,
            MileageKm = req.MileageKm,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Vehicles.Add(vehicle);
        await _db.SaveChangesAsync();

        return new VehicleResponse(vehicle.Id, vehicle.Make, vehicle.Model, vehicle.Year, vehicle.Price, vehicle.Color, vehicle.MileageKm, vehicle.IsAvailable);
    }

    // Update vehicle (Admin + OTP required)
    [HttpPut("update")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update([FromBody] VehicleUpdateRequest req)
    {
        var valid = await _otp.ValidateAsync(Guid.Parse(req.OtpId), req.Code, "updateVehicle", User.Identity!.Name!);
        if (!valid) return BadRequest(new { message = "Invalid or expired OTP" });

        var v = await _db.Vehicles.FindAsync(req.Id);
        if (v is null) return NotFound();

        v.Make = req.Make;
        v.Model = req.Model;
        v.Year = req.Year;
        v.Price = req.Price;
        v.Color = req.Color;
        v.MileageKm = req.MileageKm;

        await _db.SaveChangesAsync();
        return Ok(new VehicleResponse(v.Id, v.Make, v.Model, v.Year, v.Price, v.Color, v.MileageKm, v.IsAvailable));
    }
}
