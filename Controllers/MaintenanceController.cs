using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AutoPartsPOS.Data;
using AutoPartsPOS.Models;
using AutoPartsPOS.Models.Maintenances;
using AutoPartsPOS.Filters;

namespace AutoPartsPOS.Controllers
{
    [AuthorizeLogin]
    public class MaintenanceController : Controller
    {
        private readonly AppDbContext _context;

        public MaintenanceController(AppDbContext context)
        {
            _context = context;
        }


        // 1. عرض قائمة الصيانات
        [AuthorizePermission("Maintenance_View")]
        public IActionResult Index()
        {
            var maintenances = _context.Maintenances
                .Include(m => m.Customer)
                .Include(m => m.Technician)
                .Include(m => m.Items)         // ده السطر اللي ناقص! تحميل قطع الغيار
                .OrderByDescending(m => m.CreatedAt)
                .ToList();
            return View(maintenances);
        }

        // 2. صفحة إنشاء صيانة جديدة
        [AuthorizePermission("Maintenance_Create")]
        public IActionResult Create()
        {
            ViewBag.Customers = _context.Customers.ToList();
            ViewBag.Technicians = _context.Users.ToList(); // نفترض أن كل المستخدمين فنيين حالياً
            ViewBag.Products = _context.Products.Where(p => p.Quantity > 0).ToList();
            return View();
        }
 
        [HttpGet]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("Maintenance_Create")]

public IActionResult GetCustomerHoldItems(int customerId)
{
    var items = _context.MaintenanceHoldItems
        .Where(h => h.Maintenance.CustomerId == customerId && !h.IsDelivered)
        .Select(h => h.ItemName)
        .ToList();
    return Json(items);
}

        // 3. الأكشن الأساسي لحفظ الصيانة وتحويلها لفاتورة
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("Maintenance_Create")]
        public IActionResult Create(Maintenance model, string itemsJson ,string holdItemsJson )
        {
            // فك بيانات قطع الغيار المبعوتة من الجدول في الـ View
            var itemsDto = new List<MaintenanceItemDto>();
            if (!string.IsNullOrEmpty(itemsJson))
            {
                itemsDto = JsonSerializer.Deserialize<List<MaintenanceItemDto>>(itemsJson);
            }

            using var tx = _context.Database.BeginTransaction();
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 1;

                // --- أ. إنشاء الفاتورة ---
                var invoice = new SalesInvoice
                {
                    CustomerId = model.CustomerId,
                    InvoiceDate = DateTime.Now,
                    UserId = userId,
                    IsCredit = model.CustomerId != 1, // لو ID العميل مش 1 (نقدي) تبقى الفاتورة آجل
                    Total = 0,
                    Paid = 0
                };

                _context.SalesInvoices.Add(invoice);
                _context.SaveChanges(); // حفظ الفاتورة للحصول على الـ ID

                // --- ب. إنشاء الصيانة وربطها بالفاتورة ---
                model.CreatedAt = DateTime.Now;
                model.SalesInvoiceId = invoice.Id;
                model.Status = MaintenanceStatus.Invoiced;
                model.Notes = model.Notes ?? ""; // لو النوتس جاية null خليها نص فاضي
                _context.Maintenances.Add(model);
                _context.SaveChanges();

                decimal runningTotal = 0;

                // --- ج. إضافة بند المصنعية للفاتورة ---
                if (model.ServicePrice > 0)
                {
                    _context.SalesInvoiceItems.Add(new SalesInvoiceItem
                    {
                        InvoiceId = invoice.Id,
                        Description = $"مصنعية صيانة: {model.Name}",
                        Quantity = 1,
                        Price = model.ServicePrice,
                        ProductId = null // NULL لأنها خدمة وليست منتج
                    });
                    runningTotal += model.ServicePrice;
                }

                // --- د. معالجة قطع الغيار المضافة ---
                foreach (var item in itemsDto)
                {
                    var product = _context.Products.Find(item.ProductId);
                    if (product == null) throw new Exception("أحد المنتجات غير موجود");
                    if (product.Quantity < item.Quantity) 
                        throw new Exception($"الكمية المطلوبة من {product.Name} غير متوفرة (المتاح: {product.Quantity})");

                    // 1. خصم من المخزن
                    product.Quantity -= item.Quantity;

                    // 2. إضافة لبنود الصيانة (للتوثيق الفني)
                    _context.MaintenanceItems.Add(new MaintenanceItem
                    {
                        MaintenanceId = model.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        Price = product.SellPrice,
                        IsService = false,
                        Description = product.Name
                    });

                    // 3. إضافة لبنود الفاتورة (للحساب المالي)
                    _context.SalesInvoiceItems.Add(new SalesInvoiceItem
                    {
                        InvoiceId = invoice.Id,
                        ProductId = product.Id,
                        Description = product.Name,
                        Quantity = item.Quantity,
                        Price = product.SellPrice
                    });

                    runningTotal += (product.SellPrice * item.Quantity);
                }

                // --- هـ. تحديث الفاتورة والحسابات المالية ---
                invoice.Total = runningTotal;

                if (invoice.IsCredit)
                {
                    // العميل آجل: تحديث مديونية العميل
                    var customer = _context.Customers.Find(model.CustomerId);
                    if (customer != null)
                    {
                        customer.Balance += runningTotal;
                        _context.Customers.Update(customer);
                    }
                }
                else
                {
                    // العميل نقدي: الفاتورة مدفوعة بالكامل وتسجيل حركة خزينة
                    invoice.Paid = runningTotal;
                    _context.CashTransactions.Add(new CashTransaction
                    {
                        TransDate = DateTime.Now,
                        Amount = runningTotal,
                        TransType = "In",
                        Notes = $"تحصيل نقدي - عملية صيانة #{model.Id}",
                        SalesInvoiceId = invoice.Id,
                        CustomerId = 1,
                        UserId = userId
                    });
                }
 // --- و. معالجة الأصناف المحتجزة (الأمانات) ---
if (!string.IsNullOrEmpty(holdItemsJson))
{
    // تحويل النص الجاي من الـ View إلى قائمة نصوص
    var holdItemsNames = JsonSerializer.Deserialize<List<string>>(holdItemsJson);
    
    foreach (var name in holdItemsNames)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            _context.MaintenanceHoldItems.Add(new MaintenanceHoldItem
            {
                MaintenanceId = model.Id, // ربطها بعملية الصيانة الحالية
                ItemName = name,
                IsDelivered = false,       // الحالة الافتراضية: لم تُرد للعميل
            });
        }
    }
}


    

                _context.SaveChanges();
                tx.Commit();

                TempData["Success"] = "تم حفظ الصيانة وتوليد الفاتورة بنجاح ✔";
                return RedirectToAction("Details", new { id = model.Id });
            }
          catch (Exception ex)
{
    tx.Rollback();
    // هنا بنجيب الـ InnerException لو موجود لأنه غالباً بيكون فيه سبب رفض الـ SQL
    var innerError = ex.InnerException != null ? ex.InnerException.Message : "";
    TempData["Error"] = $"❌ فشل الحفظ: {ex.Message} {innerError}";
    
    // إعادة تحميل البيانات للـ View عشان ميرجعش بصفحة فاضية
    ViewBag.Customers = _context.Customers.ToList();
    ViewBag.Technicians = _context.Users.ToList();
    ViewBag.Products = _context.Products.Where(p => p.Quantity > 0).ToList();
    
    return View(model); 
}
        }



        // 4. تفاصيل الصيانة
        [AuthorizePermission("Maintenance_View")]
        public IActionResult Details(int id)
        {
            var maintenance = _context.Maintenances
                .Include(m => m.Customer)
                .Include(m => m.Technician)
                .Include(m => m.SalesInvoice)
                .Include(m => m.Items).ThenInclude(i => i.Product)
                .Include(m => m.HoldItems)
                .FirstOrDefault(m => m.Id == id);

            if (maintenance == null) return NotFound();

            return View(maintenance);
        }

        

        [AuthorizePermission("Maintenance_Technician_Report")]

public IActionResult TechnicianReport(int? technicianId, int? month, int? year, decimal commissionRate = 0)
{
    // الافتراضي الشهر والسنة الحالية
    int targetMonth = month ?? DateTime.Now.Month;
    int targetYear = year ?? DateTime.Now.Year;

    var query = _context.Maintenances.AsQueryable();

    if (technicianId.HasValue)
    {
        query = query.Where(m => m.TechnicianId == technicianId);
    }

    var data = query.Where(m => m.CreatedAt.Month == targetMonth && m.CreatedAt.Year == targetYear)
                    .Select(m => new { m.ServicePrice }) // نأخذ سعر المصنعية فقط
                    .ToList();

    ViewBag.TotalOperations = data.Count;
    ViewBag.TotalServiceRevenue = data.Sum(m => m.ServicePrice);
    ViewBag.CommissionRate = commissionRate;
    ViewBag.TechnicianProfit = data.Sum(m => m.ServicePrice) * (commissionRate / 100);
    
    ViewBag.Technicians = _context.Users.Where(u => u.RoleId == 10).ToList(); // تعديل حسب الـ Role عندك

    return View();
}


        // DTO داخلي لاستقبال البيانات من الـ View
        public class MaintenanceItemDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}