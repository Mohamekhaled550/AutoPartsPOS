using System;

namespace AutoPartsPOS.Models
{
    public class CashTransaction
    {
        public int Id { get; set; }
        public DateTime TransDate { get; set; }
        public decimal Amount { get; set; }

        // نوع الحركة: CashSale / CreditCollection / Return / Purchase
        public string TransType { get; set; } 

        public string Notes { get; set; }

        // ربط الفاتورة إذا كانت حركة تحصيل
        public int? SalesInvoiceId { get; set; }

        // ربط العميل لو كانت تحصيل
        public int? CustomerId { get; set; }

        // ربط المستخدم اللي قام بالتحصيل
        public int UserId { get; set; }

        // Navigation
        public SalesInvoice SalesInvoice { get; set; }
        public Customer Customer { get; set; }
        public User User { get; set; }
    }
}
