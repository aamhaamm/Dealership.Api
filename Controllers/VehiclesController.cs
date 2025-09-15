using Dealership.Api.Data;
using Dealership.Api.Dtos;
using Dealership.Api.Models;
using Dealership.Api.Services;
using Dealership.Api.Common;
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
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(AppDbContext db, IOtpService otp, ILogger<VehiclesController> logger)
    {
        _db = db;
        _otp = otp;
        _logger = logger;
    }

    [HttpGet("browse")]
    [Authorize(Policy = "CustomerOnly")]
    public async Task<IEnumerable<VehicleResponse>> Browse()
    {
        var vehicles = await _db.Vehicles.Where(v => v.IsAvailable).ToListAsync();
        return vehicles.Select(v => new VehicleResponse(
            v.Id, v.Make, v.Model, v.Year, v.Price, v.Color, v.MileageKm, v.IsAvailable
        ));
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<VehicleResponse>> GetById(Guid id)
    {
        var v = await _db.Vehicles.FindAsync(id);
        if (v is null) return NotFound(new { code = "VEHICLE_NOT_FOUND", message = "Vehicle not found" });

        return new VehicleResponse(v.Id, v.Make, v.Model, v.Year, v.Price, v.Color, v.MileageKm, v.IsAvailable);
    }

    [HttpPost("add")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<VehicleResponse>> Add([FromBody] VehicleCreateRequest req)
    {
        // Extra validation for Year (prevent adding future cars)
        if (req.Year > DateTime.UtcNow.Year)
            return BadRequest(new { code = "INVALID_YEAR", message = "Year cannot be in the future" });

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

        _logger.LogInformation("Vehicle added: {Make} {Model} ({Year})", vehicle.Make, vehicle.Model, vehicle.Year);

        return new VehicleResponse(vehicle.Id, vehicle.Make, vehicle.Model, vehicle.Year,
                                vehicle.Price, vehicle.Color, vehicle.MileageKm, vehicle.IsAvailable);
    }

    [HttpPut("update")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update([FromBody] VehicleUpdateRequest req)
    {
        var valid = await _otp.ValidateAsync(Guid.Parse(req.OtpId), req.Code, OtpPurposes.UpdateVehicle, User.Identity!.Name!);
        if (!valid) return BadRequest(new { code = "OTP_INVALID", message = "Invalid or expired OTP" });

        var v = await _db.Vehicles.FindAsync(req.Id);
        if (v is null) return NotFound(new { code = "VEHICLE_NOT_FOUND", message = "Vehicle not found" });

        v.Make = req.Make;
        v.Model = req.Model;
        v.Year = req.Year;
        v.Price = req.Price;
        v.Color = req.Color;
        v.MileageKm = req.MileageKm;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Vehicle updated: {Id}", v.Id);

        return Ok(new VehicleResponse(v.Id, v.Make, v.Model, v.Year, v.Price, v.Color, v.MileageKm, v.IsAvailable));
    }
}
