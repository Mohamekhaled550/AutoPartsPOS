namespace AutoPartsPOS.Models.Maintenances
{
    public class Maintenance
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public MaintenanceType Type { get; set; } 

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int TechnicianId { get; set; }
        public User Technician { get; set; } 

        // التعديل هنا: سعر اليد أو المصنعية
        public decimal ServicePrice { get; set; } 

        public DateTime CreatedAt { get; set; }
        public MaintenanceStatus Status { get; set; }
        public string Notes { get; set; }  

        public ICollection<MaintenanceItem> Items { get; set; }
        public ICollection<MaintenanceHoldItem> HoldItems { get; set; }

        public int? SalesInvoiceId { get; set; }
        public SalesInvoice SalesInvoice { get; set; } 
    }
}