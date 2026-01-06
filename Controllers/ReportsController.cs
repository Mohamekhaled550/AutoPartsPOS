using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoPartsPOS.Data;
using AutoPartsPOS.Models.Reports;
using AutoPartsPOS.Filters;
using System.Linq;

namespace AutoPartsPOS.Controllers
{

    [AuthorizeLogin]


    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }


        [AuthorizePermission("Products_Read")]
public async Task<IActionResult> Sales(DateTime? fromDate, DateTime? toDate, bool? isCredit)
{
    var from = fromDate ?? DateTime.Today;
    var to = toDate ?? DateTime.Today;

    var query = _context.SalesInvoices
        .Include(x => x.Customer)
        .Where(x => x.InvoiceDate.Date >= from.Date &&
                    x.InvoiceDate.Date <= to.Date);

    if (isCredit.HasValue)
        query = query.Where(x => x.IsCredit == isCredit.Value);

    var invoices = await query
        .OrderByDescending(x => x.InvoiceDate)
        .Select(x => new SalesInvoiceRow
        {
            InvoiceId = x.Id,
            InvoiceDate = x.InvoiceDate,
            CustomerName = x.Customer.Name,
            IsCredit = x.IsCredit,
            Total = x.Total
        })
        .ToListAsync();

    var vm = new SalesReportVM
    {
        FromDate = from,
        ToDate = to,
        IsCredit = isCredit,

        TotalInvoices = invoices.Count,
        TotalSales = invoices.Sum(x => x.Total),
        CashSales = invoices.Where(x => !x.IsCredit).Sum(x => x.Total),
        CreditSales = invoices.Where(x => x.IsCredit).Sum(x => x.Total),

        Invoices = invoices
    };

    return View(vm);
}


    // ================= Collections Report =================

      [AuthorizePermission("Reports_Collections")]
        public IActionResult CollectionsReport(DateTime? fromDate, DateTime? toDate, int? customerId)
        {
            var query = _context.CashTransactions
                .Include(c => c.Customer)
                .Where(c => c.TransType == "In")
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(c => c.TransDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(c => c.TransDate <= toDate.Value.Date.AddDays(1).AddTicks(-1));

            if (customerId.HasValue)
                query = query.Where(c => c.CustomerId == customerId.Value);

            var collections = query.OrderByDescending(c => c.TransDate).ToList();
            var totalCollected = collections.Sum(c => c.Amount);

            var viewModel = new CollectionsReportVM
            {
                Collections = collections,
                TotalCollected = totalCollected,
                Customers = _context.Customers.OrderBy(c => c.Name).ToList(),
                FromDate = fromDate,
                ToDate = toDate,
                SelectedCustomerId = customerId
            };

            return View(viewModel);
        }



          // ================= STOCK REPORT =================
        [AuthorizePermission("Reports_Stock")]
        public IActionResult StockReport()
        {
            var products = _context.Products
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.Name,
                    p.Type,
                    p.Quantity,
                    p.SellPrice,
                    IsLowStock = p.Quantity <= 5
                })
                .ToList();

            ViewBag.TotalItems = products.Count;
            ViewBag.TotalQuantity = products.Sum(p => p.Quantity);
            ViewBag.LowStockCount = products.Count(p => p.IsLowStock);

            return View(products);
        }


// ================= CUSTOMER STATEMENT (SELECT) =================
[AuthorizePermission("Reports_Customers")]
[HttpGet]
public IActionResult CustomerStatement()
{
    ViewBag.Customers = _context.Customers
        .Where(c => c.Id != 1004)
        .OrderBy(c => c.Name)
        .ToList();

    return View("CustomerStatementSelect");
}


// ================= CUSTOMER STATEMENT (DETAILS) =================
[AuthorizePermission("Reports_Customers")]
[HttpGet("Reports/CustomerStatement/View")]
public IActionResult CustomerStatementView(int customerId)
{
    var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);
    if (customer == null)
        return NotFound();

    var invoices = _context.SalesInvoices
        .Where(s => s.CustomerId == customerId)
        .OrderBy(s => s.InvoiceDate)
        .Select(s => new
        {
            s.Id,
            s.InvoiceDate,
            s.Total,
            s.Paid,
            Remaining = s.Total - s.Paid
        })
        .ToList();

    var collections = _context.CashTransactions
        .Where(c => c.CustomerId == customerId && c.TransType == "In")
        .OrderBy(c => c.TransDate)
        .Select(c => new
        {
            c.TransDate,
            c.Amount,
            c.SalesInvoiceId
        })
        .ToList();

    ViewBag.Customer = customer;
    ViewBag.Invoices = invoices;
    ViewBag.Collections = collections;

    ViewBag.TotalInvoices = invoices.Sum(x => x.Total);
    ViewBag.TotalCollections = collections.Sum(x => x.Amount);
    ViewBag.FinalBalance = ViewBag.TotalInvoices - ViewBag.TotalCollections;

    return View("CustomerStatement");
}
    }
    
    }