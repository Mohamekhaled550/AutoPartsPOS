using System;
using System.Collections.Generic;
using AutoPartsPOS.Models.Maintenances;

namespace AutoPartsPOS.Models
{
    public class SalesInvoice
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Total { get; set; }
        public decimal Paid { get; set; }
        public bool IsCredit { get; set; }
        public int UserId { get; set; }

        // Navigation
        public Customer Customer { get; set; }
        public User User { get; set; }

        public Maintenance Maintenance { get; set; } // One-to-One مع العملية

        public ICollection<SalesInvoiceItem> SalesInvoiceItems { get; set; }
        public ICollection<Return> Returns { get; set; }
    }
}
