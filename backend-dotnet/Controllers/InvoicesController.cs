using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvoiceGenerator.API.Data;
using InvoiceGenerator.API.DTOs;
using InvoiceGenerator.API.Services;

namespace InvoiceGenerator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly InvoiceService _invoiceService;
    private readonly PdfService _pdfService;
    private readonly EmailService _emailService;
    private readonly AppDbContext _db;

    public InvoicesController(InvoiceService invoiceService, PdfService pdfService, EmailService emailService, AppDbContext db)
    {
        _invoiceService = invoiceService;
        _pdfService = pdfService;
        _emailService = emailService;
        _db = db;
    }

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _invoiceService.GetAllAsync(UserId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var invoice = await _invoiceService.GetByIdAsync(UserId, id);
        return invoice == null ? NotFound() : Ok(invoice);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateInvoiceDto dto) =>
        Ok(await _invoiceService.CreateAsync(UserId, dto));

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateInvoiceStatusDto dto)
    {
        var updated = await _invoiceService.UpdateStatusAsync(UserId, id, dto.Status);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _invoiceService.DeleteAsync(UserId, id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> DownloadPdf(int id)
    {
        var invoice = await _invoiceService.GetByIdAsync(UserId, id);
        if (invoice == null) return NotFound();

        var user = await _db.Users.FindAsync(UserId);
        var pdfBytes = await _pdfService.GeneratePdfAsync(invoice, user!.BusinessName, user.BusinessAddress);
        return File(pdfBytes, "application/pdf", $"{invoice.InvoiceNumber}.pdf");
    }

    [HttpPost("{id}/send-email")]
    public async Task<IActionResult> SendEmail(int id)
    {
        var invoice = await _invoiceService.GetByIdAsync(UserId, id);
        if (invoice == null) return NotFound();

        var user = await _db.Users.FindAsync(UserId);
        var pdfBytes = await _pdfService.GeneratePdfAsync(invoice, user!.BusinessName, user.BusinessAddress);
        await _emailService.SendInvoiceEmailAsync(invoice.ClientEmail, invoice.ClientName, invoice.InvoiceNumber, pdfBytes);
        await _invoiceService.UpdateStatusAsync(UserId, id, "Sent");

        return Ok(new { message = "Invoice sent successfully." });
    }
}
