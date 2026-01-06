namespace AutoPartsPOS.Models
{
    public class SalesInvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Navigation
        public SalesInvoice SalesInvoice { get; set; }
        public Product Product { get; set; }
    }
}
