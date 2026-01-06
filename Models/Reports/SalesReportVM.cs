namespace AutoPartsPOS.Models.Reports
{
    public class SalesReportVM
    {
        // Filters
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool? IsCredit { get; set; } // null = الكل

        // Summary
        public int TotalInvoices { get; set; }
        public decimal TotalSales { get; set; }
        public decimal CashSales { get; set; }
        public decimal CreditSales { get; set; }

        // Details
        public List<SalesInvoiceRow> Invoices { get; set; } = new();
    }

    public class SalesInvoiceRow
    {
        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public bool IsCredit { get; set; }
        public decimal Total { get; set; }
    }
}
