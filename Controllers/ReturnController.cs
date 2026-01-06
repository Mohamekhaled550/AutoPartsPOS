using AutoPartsPOS.Data;
using AutoPartsPOS.Models;
using AutoPartsPOS.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

[AuthorizeLogin]
[AuthorizePermission("Returns_Create")]
public class ReturnsController : Controller
{
    private readonly AppDbContext _context;

    public ReturnsController(AppDbContext context)
    {
        _context = context;
    }

    // ================= SELECT TYPE =================
    public IActionResult Index()
    {
        return View();
    }

    // ================= LIST SALES INVOICES =================
    public IActionResult SalesInvoices()
    {
        var invoices = _context.SalesInvoices
            .Include(i => i.Customer)
            .Include(i => i.SalesInvoiceItems)
            .ThenInclude(si => si.Product)
            .Where(i => i.Total > 0)
            .OrderByDescending(i => i.InvoiceDate)
            .ToList();

        return View(invoices);
    }

    // ================= LIST PURCHASE INVOICES =================
    public IActionResult PurchaseInvoices()
    {
        var invoices = _context.PurchaseInvoices
            .Include(i => i.Supplier)
            .Include(i => i.PurchaseInvoiceItems)
            .ThenInclude(pi => pi.Product)
            .Where(i => i.Total > 0)
            .OrderByDescending(i => i.InvoiceDate)
            .ToList();

        return View(invoices);
    }

    // ================= SALES RETURN =================
    public IActionResult Sales(int invoiceId)
    {
        var invoice = _context.SalesInvoices
            .Include(i => i.SalesInvoiceItems)
            .ThenInclude(i => i.Product)
            .FirstOrDefault(i => i.Id == invoiceId);

        if (invoice == null)
            return NotFound();

        return View(invoice);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SalesConfirm(int invoiceId, int[] quantities)
    {
        var invoice = _context.SalesInvoices
            .Include(i => i.SalesInvoiceItems)
            .ThenInclude(i => i.Product)
            .FirstOrDefault(i => i.Id == invoiceId);

        if (invoice == null)
            return NotFound();

        var items = invoice.SalesInvoiceItems.ToList(); // تحويل ICollection لقائمة لتجنب CS0021

        using var tx = _context.Database.BeginTransaction();
        try
        {
            var ret = new Return
            {
                ReturnType = ReturnType.Sales,
                SalesInvoiceId = invoice.Id,
                CustomerId = invoice.CustomerId,
                ReturnDate = DateTime.Now,
                Total = 0
            };

            _context.Returns.Add(ret);
            _context.SaveChanges();

            for (int i = 0; i < items.Count; i++)
            {
                if (quantities[i] <= 0) continue;

                var item = items[i];

                _context.ReturnItems.Add(new ReturnItem
                {
                    ReturnId = ret.Id,
                    ProductId = item.ProductId,
                    Quantity = quantities[i],
                    Price = item.Price
                });

                item.Product.Quantity += quantities[i];
                invoice.Total -= item.Price * quantities[i];
                ret.Total += item.Price * quantities[i];
            }

            invoice.Paid = Math.Min(invoice.Paid, invoice.Total);

            var customer = _context.Customers.Find(invoice.CustomerId);
            customer.Balance -= ret.Total;

            _context.SaveChanges();
            tx.Commit();

            TempData["Success"] = "تم تسجيل مردود البيع ✔";
            return RedirectToAction("SalesInvoices");
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    // ================= PURCHASE RETURN =================
    public IActionResult Purchase(int invoiceId)
    {
        var invoice = _context.PurchaseInvoices
            .Include(i => i.PurchaseInvoiceItems)
            .ThenInclude(i => i.Product)
            .FirstOrDefault(i => i.Id == invoiceId);

        if (invoice == null)
            return NotFound();

        return View(invoice);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PurchaseConfirm(int invoiceId, int[] quantities)
    {
        var invoice = _context.PurchaseInvoices
            .Include(i => i.PurchaseInvoiceItems)
            .ThenInclude(i => i.Product)
            .FirstOrDefault(i => i.Id == invoiceId);

        if (invoice == null)
            return NotFound();

        var items = invoice.PurchaseInvoiceItems.ToList(); // تحويل ICollection لقائمة

        using var tx = _context.Database.BeginTransaction();
        try
        {
            var ret = new Return
            {
                ReturnType = ReturnType.Purchase,
                PurchaseInvoiceId = invoice.Id,
                SupplierId = invoice.SupplierId,
                ReturnDate = DateTime.Now,
                Total = 0
            };

            _context.Returns.Add(ret);
            _context.SaveChanges();

            for (int i = 0; i < items.Count; i++)
            {
                if (quantities[i] <= 0) continue;

                var item = items[i];

                _context.ReturnItems.Add(new ReturnItem
                {
                    ReturnId = ret.Id,
                    ProductId = item.ProductId,
                    Quantity = quantities[i],
                    Price = item.Price
                });

                item.Product.Quantity -= quantities[i];
                invoice.Total -= item.Price * quantities[i];
                ret.Total += item.Price * quantities[i];
            }

            var supplier = _context.Suppliers.Find(invoice.SupplierId);
            supplier.Balance -= ret.Total;

            _context.SaveChanges();
            tx.Commit();

            TempData["Success"] = "تم تسجيل مردود الشراء ✔";
            return RedirectToAction("PurchaseInvoices");
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}
