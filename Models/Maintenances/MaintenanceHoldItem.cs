using System;

namespace AutoPartsPOS.Models.Maintenances
{
    public class MaintenanceHoldItem
    {
        public int Id { get; set; }

        public int MaintenanceId { get; set; }
        public Maintenance Maintenance { get; set; }

        public string ItemName { get; set; } // اسم القطعة
        public DateTime ReceivedAt { get; set; }

        public bool IsDelivered { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
