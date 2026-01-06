using Microsoft.AspNetCore.Mvc;
using AutoPartsPOS.Data;
using AutoPartsPOS.Models;
using AutoPartsPOS.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsPOS.Controllers
{
    [AuthorizeLogin]
    public class SalesController : Controller
    {
        private readonly AppDbContext _context;

        public SalesController(AppDbContext context)
        {
            _context = context;
        }

        // ================= POS =================
        [AuthorizePermission("SalesInvoices_Create")]
        public IActionResult Pos()
        {
            return LoadPos();
        }

        // ================= CREATE =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("SalesInvoices_Create")]
        public IActionResult Create(string invoiceItemsJson, bool IsCredit, int? CustomerId)
        {
            if (string.IsNullOrWhiteSpace(invoiceItemsJson))
                return Error("لم يتم إرسال بيانات الفاتورة");

            var items = JsonSerializer.Deserialize<List<SalesItemDto>>(invoiceItemsJson);
            if (items == null || !items.Any())
                return Error("يجب إضافة صنف واحد على الأقل");

            if (IsCredit && !CustomerId.HasValue)
                return Error("يجب اختيار العميل في البيع الآجل");

            using var tx = _context.Database.BeginTransaction();
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                    return Error("انتهت الجلسة، برجاء تسجيل الدخول مرة أخرى");
                // إنشاء الفاتورة


                var invoice = new SalesInvoice
                {
                    CustomerId = IsCredit ? CustomerId : 1004,
                    InvoiceDate = DateTime.Now,
                    IsCredit = IsCredit,
                    Total = 0,
                    Paid = 0,
                    UserId = userId.Value
                };
                _context.SalesInvoices.Add(invoice);
                _context.SaveChanges();

                // إضافة الأصناف وتحديث المخزون
                foreach (var i in items)
                {
                    if (i.Quantity <= 0) throw new Exception("كمية غير صحيحة");

                    var product = _context.Products.FirstOrDefault(p => p.Id == i.ProductId);
                    if (product == null) throw new Exception("منتج غير موجود");
                    if (product.Quantity < i.Quantity)
                        throw new Exception($"الكمية غير متوفرة: {product.Name}");

                    var price = product.SellPrice;
                    _context.SalesInvoiceItems.Add(new SalesInvoiceItem
                    {
                        InvoiceId = invoice.Id,
                        ProductId = product.Id,
                        Quantity = i.Quantity,
                        Price = price
                    });

                    invoice.Total += price * i.Quantity;
                    product.Quantity -= i.Quantity;
                    _context.Products.Update(product);
                }

                // ================= التعامل مع النقدي / آجل =================
                if (!IsCredit)
                {
                    invoice.Paid = invoice.Total;

                    _context.CashTransactions.Add(new CashTransaction
                    {
                        TransDate = DateTime.Now,
                        Amount = invoice.Total,
                        TransType = "In",
                        Notes = $"فاتورة بيع رقم {invoice.Id}",
                        CustomerId =1004,
                        SalesInvoiceId = invoice.Id,
                        UserId = userId.Value
                    });
                }
                else
                {
                    // آجل
                    var customer = _context.Customers.Find(CustomerId);
                    customer.Balance += invoice.Total;
                    _context.Customers.Update(customer);
                }

                _context.SalesInvoices.Update(invoice);
                _context.SaveChanges();
                tx.Commit();

                TempData["Success"] = "تمت عملية البيع بنجاح ✔";
                return RedirectToAction("Pos");
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return Error(ex.InnerException?.Message ?? ex.Message);
            }
        }

        // ================= PRIVATE =================
        private IActionResult LoadPos()
        {
            ViewBag.Products = _context.Products
                .Where(p => p.Quantity > 0)
                .OrderBy(p => p.Name)
                .ToList();

            ViewBag.Customers = _context.Customers
                .OrderBy(c => c.Name)
                .Where(c => c.Id != 1004) // استبعاد العميل الافتراضي
                .ToList();

            return View("Pos");
        }

        private IActionResult Error(string msg)
        {
            TempData["Error"] = msg;
            return LoadPos();
        }

        // DTO
        public class SalesItemDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
