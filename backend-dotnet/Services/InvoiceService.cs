using Microsoft.EntityFrameworkCore;
using InvoiceGenerator.API.Data;
using InvoiceGenerator.API.DTOs;
using InvoiceGenerator.API.Models;

namespace InvoiceGenerator.API.Services;

public class InvoiceService
{
    private readonly AppDbContext _db;

    public InvoiceService(AppDbContext db) => _db = db;

    public async Task<InvoiceResponseDto> CreateAsync(int userId, CreateInvoiceDto dto)
    {
        var count = _db.Invoices.Count(i => i.UserId == userId) + 1;
        var invoice = new Invoice
        {
            UserId = userId,
            ClientId = dto.ClientId,
            InvoiceNumber = $"INV-{userId}-{count:D4}",
            IssueDate = dto.IssueDate,
            DueDate = dto.DueDate,
            Notes = dto.Notes,
            Tax = dto.Tax,
            Discount = dto.Discount,
            Status = "Draft",
            Items = dto.Items.Select(i => new InvoiceItem
            {
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();
        return await GetByIdAsync(userId, invoice.Id) ?? throw new Exception("Failed to retrieve created invoice");
    }

    public async Task<List<InvoiceResponseDto>> GetAllAsync(int userId)
    {
        var invoices = await _db.Invoices
            .Include(i => i.Client)
            .Include(i => i.Items)
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return invoices.Select(MapToDto).ToList();
    }

    public async Task<InvoiceResponseDto?> GetByIdAsync(int userId, int invoiceId)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Client)
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == invoiceId && i.UserId == userId);

        return invoice == null ? null : MapToDto(invoice);
    }

    public async Task<bool> UpdateStatusAsync(int userId, int invoiceId, string status)
    {
        var invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId && i.UserId == userId);
        if (invoice == null) return false;
        invoice.Status = status;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int userId, int invoiceId)
    {
        var invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId && i.UserId == userId);
        if (invoice == null) return false;
        _db.Invoices.Remove(invoice);
        await _db.SaveChangesAsync();
        return true;
    }

    private static InvoiceResponseDto MapToDto(Invoice i)
    {
        var subtotal = i.Items.Sum(x => x.Quantity * x.UnitPrice);
        var total = subtotal + i.Tax - i.Discount;
        return new InvoiceResponseDto(
            i.Id, i.InvoiceNumber, i.Client.Name, i.Client.Email,
            i.IssueDate, i.DueDate, i.Status,
            subtotal, i.Tax, i.Discount, total, i.Notes, i.CreatedAt,
            i.Items.Select(x => new InvoiceItemResponseDto(
                x.Id, x.Description, x.Quantity, x.UnitPrice, x.Quantity * x.UnitPrice
            )).ToList()
        );
    }
}
