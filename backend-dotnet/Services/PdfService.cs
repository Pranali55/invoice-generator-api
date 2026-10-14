using System.Text;
using System.Text.Json;
using InvoiceGenerator.API.DTOs;

namespace InvoiceGenerator.API.Services;

public class PdfService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public PdfService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<byte[]> GeneratePdfAsync(InvoiceResponseDto invoice, string businessName, string? businessAddress)
    {
        var payload = new
        {
            invoice_number = invoice.InvoiceNumber,
            business_name = businessName,
            business_address = businessAddress,
            client_name = invoice.ClientName,
            client_email = invoice.ClientEmail,
            issue_date = invoice.IssueDate.ToString("yyyy-MM-dd"),
            due_date = invoice.DueDate.ToString("yyyy-MM-dd"),
            items = invoice.Items.Select(i => new
            {
                description = i.Description,
                quantity = i.Quantity,
                unit_price = i.UnitPrice,
                line_total = i.LineTotal
            }),
            subtotal = invoice.Subtotal,
            tax = invoice.Tax,
            discount = invoice.Discount,
            total = invoice.Total,
            notes = invoice.Notes
        };

        var baseUrl = _config["PdfService__BaseUrl"];
        var response = await _http.PostAsync(
            $"{baseUrl}/generate-pdf",
            new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        );

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }
}
