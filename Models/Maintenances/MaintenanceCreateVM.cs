using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoPartsPOS.Models.Maintenances;

namespace AutoPartsPOS.Models.Maintenances
{
    public class MaintenanceItemVM
    {
        public string Description { get; set; }

        public bool IsService { get; set; }

        public int? ProductId { get; set; }

        public int Quantity { get; set; } = 1;

        public decimal Price { get; set; } = 0;
    }

    public class MaintenanceHoldItemVM
    {
        public string ItemName { get; set; }
    }

    public class MaintenanceCreateVM
    {
        [Required(ErrorMessage = "اسم العملية مطلوب")]
        public string Name { get; set; }

        [Required(ErrorMessage = "نوع الصيانة مطلوب")]
        public MaintenanceType? Type { get; set; }

        [Required(ErrorMessage = "العميل مطلوب")]
        public int? CustomerId { get; set; }

        [Required(ErrorMessage = "الفني مطلوب")]
        public int? TechnicianId { get; set; }

        public string Notes { get; set; }

        public List<MaintenanceItemVM> Items { get; set; } = new List<MaintenanceItemVM>();
        public List<MaintenanceHoldItemVM> HoldItems { get; set; } = new List<MaintenanceHoldItemVM>();

        public bool CreateInvoice { get; set; }
        public bool IsCredit { get; set; }
    }
}
