using System.Collections.Generic;

namespace AutoPartsPOS.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public decimal Balance { get; set; }

        // Navigation
        public ICollection<SalesInvoice> ?SalesInvoices { get; set; }
    }
}
