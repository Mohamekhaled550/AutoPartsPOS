using Microsoft.AspNetCore.Mvc;
using AutoPartsPOS.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoPartsPOS.Filters;


namespace AutoPartsPOS.Controllers
{
        [AuthorizeLogin]

    public class HomeController : Controller
    {

       
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Cards
            ViewBag.TotalProducts = _context.Products.Count();
            ViewBag.TotalCustomers = _context.Customers.Count();
            ViewBag.TotalSuppliers = _context.Suppliers.Count();
            ViewBag.TotalSalesToday = _context.SalesInvoices
                                        .Where(s => s.InvoiceDate.Date == System.DateTime.Today)
                                        .Sum(s => (decimal?)s.Total) ?? 0;
            ViewBag.TotalPurchasesToday = _context.PurchaseInvoices
                                        .Where(p => p.InvoiceDate.Date == System.DateTime.Today)
                                        .Sum(p => (decimal?)p.Total) ?? 0;

           ViewBag.RecentSales = _context.SalesInvoices
    .Include(s => s.Customer)
    .OrderByDescending(s => s.InvoiceDate)
    .Take(5)
    .Select(s => new
    {
        s.Id,
        s.InvoiceDate,
        s.Total,
        s.IsCredit,
        CustomerName = s.Customer != null ? s.Customer.Name : "نقدي"
    })
    .ToList();


            // آخر 5 فواتير مشتريات
            ViewBag.RecentPurchases = _context.PurchaseInvoices
                                        .Include(p => p.Supplier)
                                        .OrderByDescending(p => p.InvoiceDate)
                                        .Take(5)
                                        .ToList();

            return View();
        }
    }
}
