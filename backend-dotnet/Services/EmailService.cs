using MailKit.Net.Smtp;
using MimeKit;

namespace InvoiceGenerator.API.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config) => _config = config;

    public async Task SendInvoiceEmailAsync(string toEmail, string toName, string invoiceNumber, byte[] pdfBytes)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Invoice Generator", _config["Smtp__User"]));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = $"Invoice {invoiceNumber}";

        var builder = new BodyBuilder
        {
            HtmlBody = $@"
                <h2>Invoice {invoiceNumber}</h2>
                <p>Dear {toName},</p>
                <p>Please find your invoice attached.</p>
                <p>Thank you for your business!</p>"
        };

        builder.Attachments.Add($"{invoiceNumber}.pdf", pdfBytes, ContentType.Parse("application/pdf"));
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_config["Smtp__Host"], int.Parse(_config["Smtp__Port"] ?? "587"), false);
        await client.AuthenticateAsync(_config["Smtp__User"], _config["Smtp__Pass"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
