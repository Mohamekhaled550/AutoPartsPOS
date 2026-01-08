using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using AutoPartsPOS.Models.Maintenances;
namespace AutoPartsPOS.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [MaxLength(255)] // إضافة حد أقصى للحجم لضمان سلامة قاعدة البيانات
        public string Name { get; set; }

        [Required(ErrorMessage = "نوع المنتج مطلوب")]
        public string ?Type { get; set; }

        [Required(ErrorMessage = "سعر الشراء مطلوب")] // تم التحديث: إضافة Required
        [Range(0.01, 1000000.00, ErrorMessage = "سعر الشراء غير صحيح (يجب أن يكون أكبر من صفر)")]
        public decimal BuyPrice { get; set; }

        [Required(ErrorMessage = "سعر البيع مطلوب")] // تم التحديث: إضافة Required
        [Range(0.01, 1000000.00, ErrorMessage = "سعر البيع غير صحيح (يجب أن يكون أكبر من صفر)")]
        public decimal SellPrice { get; set; }

        [Required(ErrorMessage = "الكمية مطلوبة")] // تم التحديث: إضافة Required
        [Range(0, int.MaxValue, ErrorMessage = "الكمية غير صحيحة")]
        public int Quantity { get; set; }


        // Navigation Properties
        public ICollection<SalesInvoiceItem> ?SalesInvoiceItems { get; set; }
         public ICollection<MaintenanceItem> ? MaintenanceItems { get; set; }

    }
}