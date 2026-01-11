using Microsoft.AspNetCore.Mvc;
using AutoPartsPOS.Data;
using AutoPartsPOS.Models;
using AutoPartsPOS.Filters;
using System;
using System.Linq;

namespace AutoPartsPOS.Controllers
{
    [AuthorizeLogin]
    public class SafeController : Controller
    {
        private readonly AppDbContext _context;

        public SafeController(AppDbContext context)
        {
            _context = context;
        }

        // عرض سجل الخزنة والرصيد الحالي
        [AuthorizePermission("CashTransactions_Read")]
        public IActionResult Index()
        {
            var transactions = _context.CashTransactions
                .OrderByDescending(t => t.TransDate)
                .ToList();

            // حساب الرصيد: الوارد (In) ناقص الصادر (Out)
            decimal totalIn = transactions.Where(t => t.TransType == "In").Sum(t => t.Amount);
            decimal totalOut = transactions.Where(t => t.TransType == "Out").Sum(t => t.Amount);
            
            ViewBag.Balance = totalIn - totalOut;
            ViewBag.TotalIn = totalIn;
            ViewBag.TotalOut = totalOut;

            return View(transactions);
        }

        // عملية السحب النقدي
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("CashTransactions_Create")]
        public IActionResult Withdraw(decimal amount, string notes)
        {
            if (amount <= 0)
            {
                TempData["Error"] = "المبلغ يجب أن يكون أكبر من صفر";
                return RedirectToAction("Index");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            
            var transaction = new CashTransaction
            {
                TransDate = DateTime.Now,
                Amount = amount,
                TransType = "Out", // تحديد أنها حركة خروج
                Notes = "سحب نقدي: " + notes,
                UserId = userId ?? 0
            };

            _context.CashTransactions.Add(transaction);
            _context.SaveChanges();

            TempData["Success"] = "تم تسجيل عملية السحب بنجاح ✔";
            return RedirectToAction("Index");
        }
    }
}