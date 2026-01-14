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

    public decimal ReturnedServicePrice { get; set; }

    public string Reason { get; set; } // ضروري جداً للأدمن يعرف ليه العملية رجعت

    // Navigation
    public SalesInvoice SalesInvoice { get; set; }
    public Customer Customer { get; set; }

    public ICollection<ReturnItem> Items { get; set; }
}

}