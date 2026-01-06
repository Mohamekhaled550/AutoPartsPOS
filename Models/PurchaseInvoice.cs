using System;
using System.Collections.Generic;

namespace AutoPartsPOS.Models
{
    public class PurchaseInvoice
    {
        public int Id { get; set; }
        public int? SupplierId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Total { get; set; }
        public decimal Paid { get; set; }

        // Navigation
        public Supplier Supplier { get; set; }
        public ICollection<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; }
        public ICollection<Return> Returns { get; set; }
    }
}
