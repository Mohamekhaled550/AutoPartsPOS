using AutoPartsPOS.Models.Reports;
using System;
using System.Collections.Generic;

namespace AutoPartsPOS.Models.Reports
{
    public class CollectionsReportVM
    {
        public List<CashTransaction> Collections { get; set; } = new();
        public decimal TotalCollected { get; set; }
        public List<Customer> Customers { get; set; } = new();
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? SelectedCustomerId { get; set; }
    }
}
