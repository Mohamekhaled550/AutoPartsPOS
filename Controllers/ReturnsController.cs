using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoPartsPOS.Data;
using AutoPartsPOS.Models;
using AutoPartsPOS.Filters;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AutoPartsPOS.Controllers
{
    [AuthorizeLogin]
    public class ReturnsController : Controller
    {
        private readonly AppDbContext _context;

        public ReturnsController(AppDbContext context)
        {
            _context = context;
        }

        // عرض قائمة المرتجعات السابقة وتحميل العملاء
        public IActionResult Index()
        {
            var data = _context.Returns
                .Include(r => r.Customer)
                .Include(r => r.SalesInvoice)
                .OrderByDescending(r => r.ReturnDate)
                .ToList();

            ViewBag.Customers = _context.Customers.ToList();
            return View(data);
        }

        // جلب فواتير عميل معين - AJAX
        [HttpGet]
        public IActionResult GetInvoicesByCustomer(int customerId)
        {
            var invoices = _context.SalesInvoices
                .Where(i => i.CustomerId == customerId)
                .OrderByDescending(i => i.Id)
                .Select(i => new {
                    id = i.Id,
                    total = i.Total,
                    date = i.InvoiceDate.ToString("yyyy/MM/dd")
                }).ToList();

            return Json(invoices);
        }

        // جلب أصناف فاتورة معينة - AJAX
        [HttpGet]
        public IActionResult GetInvoiceItems(int id)
        {
            var items = _context.SalesInvoiceItems
                .Where(i => i.InvoiceId == id)
                .Select(i => new {
                    productId = i.ProductId,
                    description = i.Description,
                    quantity = i.Quantity,
                    price = i.Price
                }).ToList();

            return Json(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateReturn(int invoiceId, List<ReturnItem> items, decimal returnServiceAmount, string reason)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var invoice = _context.SalesInvoices.Find(invoiceId);
                if (invoice == null) return BadRequest("الفاتورة غير موجودة");

                decimal totalReturnMoney = 0;
                var salesReturn = new Return {
                    SalesInvoiceId = invoiceId,
                    CustomerId = invoice.CustomerId,
                    ReturnDate = DateTime.Now,
                    ReturnType = ReturnType.Sales,
                    Items = new List<ReturnItem>(),
                    ReturnedServicePrice = returnServiceAmount,
                    Total = 0 // سيتم تحديثه
                };

                if (items != null)
                {
                    foreach (var item in items.Where(x => x.Quantity > 0))
                    {
                        var product = _context.Products.Find(item.ProductId);
                        if (product != null)
                        {
                            product.Quantity += item.Quantity; // إعادة للمخزن
                            _context.Products.Update(product);
                        }

                        totalReturnMoney += (item.Price * item.Quantity);
                        salesReturn.Items.Add(new ReturnItem {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            Price = item.Price
                        });
                    }
                }

                totalReturnMoney += returnServiceAmount;
                salesReturn.Total = totalReturnMoney;

                // تحديث الخزينة
                _context.CashTransactions.Add(new CashTransaction {
                    TransDate = DateTime.Now,
                    Amount = totalReturnMoney,
                    TransType = "Out",
                    Notes = $"مرتجع فاتورة #{invoiceId}: {reason}",
                    SalesInvoiceId = invoiceId,
                    UserId = HttpContext.Session.GetInt32("UserId") ?? 1,
                    CustomerId = invoice.CustomerId ?? 1
                });

                // تحديث مديونية العميل إذا كانت الفاتورة آجل
                if (invoice.IsCredit && invoice.CustomerId.HasValue)
                {
                    var customer = _context.Customers.Find(invoice.CustomerId);
                    if (customer != null)
                    {
                        customer.Balance -= totalReturnMoney;
                        _context.Customers.Update(customer);
                    }
                }

                _context.Returns.Add(salesReturn);
                _context.SaveChanges();
                transaction.Commit();

                TempData["Success"] = "تمت عملية المرتجع بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return BadRequest("حدث خطأ: " + ex.Message);
            }
        }
    }
}