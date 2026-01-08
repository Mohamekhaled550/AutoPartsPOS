namespace AutoPartsPOS.Models.Maintenances
{
    public class MaintenanceItem
    {
        public int Id { get; set; }

        public int MaintenanceId { get; set; }
        public Maintenance Maintenance { get; set; }

        public string Description { get; set; } // وصف الصيانة أو القطعة

        public int? ProductId { get; set; } // لو قطعة غيار
        public Product Product { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public bool IsService { get; set; } // ✔ صيانة / ❌ قطعة
    }
}
