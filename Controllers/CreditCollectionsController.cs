using Microsoft.AspNetCore.Mvc;
using AutoPartsPOS.Data;
using AutoPartsPOS.Models;
using AutoPartsPOS.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AutoPartsPOS.Controllers
{
    [AuthorizeLogin]
    public class CreditCollectionsController : Controller
    {
        private readonly AppDbContext _context;

        public CreditCollectionsController(AppDbContext context)
        {
            _context = context;
        }

        [AuthorizePermission("CashTransactions_Read")]
        public IActionResult Index()
        {
            var invoices = _context.SalesInvoices
                .Include(s => s.Customer)
                .Where(s => s.IsCredit && s.Total > s.Paid)
                .OrderByDescending(s => s.InvoiceDate)
                .ToList();

            return View(invoices);
        }

        [HttpPost]
        [AuthorizePermission("CashTransactions_Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Collect(int invoiceId, decimal amount)
        {
            var invoice = _context.SalesInvoices
                .Include(s => s.Customer)
                .FirstOrDefault(s => s.Id == invoiceId);

            if (invoice == null)
                return RedirectWithError("الفاتورة غير موجودة");

            if (amount <= 0)
                return RedirectWithError("المبلغ غير صحيح");

            if (amount > (invoice.Total - invoice.Paid))
                return RedirectWithError("المبلغ أكبر من المتبقي");

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectWithError("حدث خطأ: المستخدم غير معرف");

            using var tx = _context.Database.BeginTransaction();
            try
            {
                invoice.Paid += amount;
                _context.SalesInvoices.Update(invoice);

                // خصم من رصيد العميل
                if (invoice.Customer != null)
                {
                    invoice.Customer.Balance -= amount;
                    if (invoice.Customer.Balance < 0)
                        invoice.Customer.Balance = 0;

                    _context.Customers.Update(invoice.Customer);
                }

                // تسجيل حركة نقدية
                _context.CashTransactions.Add(new CashTransaction
                {
                    TransDate = DateTime.Now,
                    Amount = amount,
                    TransType = "In",
                    Notes = $"تحصيل من فاتورة #{invoice.Id} للعميل {invoice.Customer?.Name}",
                    SalesInvoiceId = invoice.Id,
                    CustomerId = invoice.CustomerId,
                    UserId = userId.Value
                });

                _context.SaveChanges();
                tx.Commit();

                TempData["Success"] = "تم التحصيل بنجاح ✔";
            }
            catch (Exception ex)
            {
                tx.Rollback();
                TempData["Error"] = ex.InnerException?.Message ?? ex.Message;
            }

            return RedirectToAction("Index");
        }

        private IActionResult RedirectWithError(string msg)
        {
            TempData["Error"] = msg;
            return RedirectToAction("Index");
        }
    }
}
