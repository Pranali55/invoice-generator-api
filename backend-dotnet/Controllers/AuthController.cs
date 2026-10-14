using Microsoft.AspNetCore.Mvc;
using InvoiceGenerator.API.DTOs;
using InvoiceGenerator.API.Services;

namespace InvoiceGenerator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;
    public AuthController(AuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _auth.RegisterAsync(dto);
        if (result == null) return BadRequest("Email already exists.");
        return Ok(result);
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        var result = _auth.Login(dto);
        if (result == null) return Unauthorized("Invalid credentials.");
        return Ok(result);
    }
}
