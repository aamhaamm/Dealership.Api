using Dealership.Api.Dtos;
using Dealership.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dealership.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OtpController : ControllerBase
{
    private readonly IOtpService _otp;

    public OtpController(IOtpService otp)
    {
        _otp = otp;
    }

    // Request new OTP
    [HttpPost("request")]
    public async Task<IActionResult> RequestOtp([FromBody] OtpRequest req)
    {
        var token = await _otp.GenerateAsync(req.Purpose, req.Identifier, TimeSpan.FromMinutes(5));
        return Ok(new { token.Id, token.ExpiresAtUtc });
    }

    // Verify OTP
    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] OtpVerify req, [FromQuery] string purpose = "", [FromQuery] string identifier = "")
    {
        var ok = await _otp.ValidateAsync(Guid.Parse(req.OtpId), req.Code, purpose, identifier);
        if (!ok) return BadRequest(new { message = "Invalid or expired OTP" });
        return Ok(new { message = "OTP valid" });
    }
}
