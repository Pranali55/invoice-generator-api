namespace InvoiceGenerator.API.DTOs;

// Auth
public record RegisterDto(string Name, string Email, string Password, string BusinessName, string? BusinessAddress, string? Phone);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(string Token, string Name, string Email, string BusinessName);

// Client
public record CreateClientDto(string Name, string Email, string? Phone, string? Address);
public record ClientResponseDto(int Id, string Name, string Email, string? Phone, string? Address, DateTime CreatedAt);

// Invoice
public record CreateInvoiceDto(
    int ClientId,
    DateTime IssueDate,
    DateTime DueDate,
    string? Notes,
    decimal Tax,
    decimal Discount,
    List<CreateInvoiceItemDto> Items
);

public record CreateInvoiceItemDto(string Description, int Quantity, decimal UnitPrice);

public record InvoiceResponseDto(
    int Id,
    string InvoiceNumber,
    string ClientName,
    string ClientEmail,
    DateTime IssueDate,
    DateTime DueDate,
    string Status,
    decimal Subtotal,
    decimal Tax,
    decimal Discount,
    decimal Total,
    string? Notes,
    DateTime CreatedAt,
    List<InvoiceItemResponseDto> Items
);

public record InvoiceItemResponseDto(int Id, string Description, int Quantity, decimal UnitPrice, decimal LineTotal);

public record UpdateInvoiceStatusDto(string Status);
