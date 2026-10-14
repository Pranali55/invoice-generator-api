using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvoiceGenerator.API.Data;
using InvoiceGenerator.API.DTOs;
using InvoiceGenerator.API.Models;

namespace InvoiceGenerator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ClientsController(AppDbContext db) => _db = db;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _db.Clients
            .Where(c => c.UserId == UserId)
            .Select(c => new ClientResponseDto(c.Id, c.Name, c.Email, c.Phone, c.Address, c.CreatedAt))
            .ToListAsync();
        return Ok(clients);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClientDto dto)
    {
        var client = new Client
        {
            UserId = UserId,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address
        };
        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        return Ok(new ClientResponseDto(client.Id, client.Name, client.Email, client.Phone, client.Address, client.CreatedAt));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.Id == id && c.UserId == UserId);
        if (client == null) return NotFound();
        _db.Clients.Remove(client);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
