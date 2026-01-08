namespace AutoPartsPOS.Models
{
    public class SalesInvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public SalesInvoice SalesInvoice { get; set; }

        // التعديل هنا: الـ ProductId يبقى Nullable عشان المصنعية
        public int? ProductId { get; set; } 
        public Product Product { get; set; }

        // إضافة حقل الوصف عشان نكتب فيه "مصنعية صيانة" أو اسم القطعة
        public string Description { get; set; } 

        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // إجمالي السطر (حقل محسوب)
        public decimal SubTotal => Quantity * Price;
    }
}