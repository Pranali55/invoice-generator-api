namespace InvoiceGenerator.API.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string? BusinessAddress { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Client> Clients { get; set; } = new List<Client>();
}

public class Client
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

public class Invoice
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Sent, Paid, Overdue
    public string? Notes { get; set; }
    public decimal Tax { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}

public class InvoiceItem
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public Invoice Invoice { get; set; } = null!;
}
