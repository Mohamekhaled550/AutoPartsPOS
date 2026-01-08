using System;

namespace AutoPartsPOS.Models
{
    
    public class Return
{
    
    public int Id { get; set; }   
    
    public ReturnType ReturnType { get; set; }

    public int? SalesInvoiceId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime ReturnDate { get; set; }
    public decimal Total { get; set; }

    // Navigation
    public SalesInvoice SalesInvoice { get; set; }
    public Customer Customer { get; set; }

    public ICollection<ReturnItem> Items { get; set; }
}

}