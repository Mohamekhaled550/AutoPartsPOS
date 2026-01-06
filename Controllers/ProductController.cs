using Microsoft.AspNetCore.Mvc;
using AutoPartsPOS.Data;
using AutoPartsPOS.Models;
using AutoPartsPOS.Filters;
using System.Linq;
using Microsoft.EntityFrameworkCore; // تمت الإضافة لجلب البيانات بشكل أفضل في المستقبل

namespace AutoPartsPOS.Controllers
{

    [AuthorizeLogin]
    public class ProductsController : Controller
    {

        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Products
        [AuthorizePermission("Products_Read")]
        public IActionResult Index()
        {
            // قد تحتاج مستقبلاً لاستخدام Include لجلب العلاقات إذا لزم الأمر
            var products = _context.Products.ToList();
            return View(products);
        }

        // GET: /Products/Create
        [AuthorizePermission("Products_Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Products/Create
        [HttpPost]
        [AuthorizePermission("Products_Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product model)
        {
             if (!ModelState.IsValid)
    {
        // 👇 اطبع الأخطاء في الكونسول
        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                Console.WriteLine($"FIELD: {state.Key} => ERROR: {error.ErrorMessage}");
            }
        }

        return View(model);
    }

    _context.Products.Add(model);
    _context.SaveChanges();

    return RedirectToAction(nameof(Index));
}
        

        // GET: /Products/Edit/{id}
        [AuthorizePermission("Products_Update")]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: /Products/Edit
        [HttpPost]
        [AuthorizePermission("Products_Update")]
        [ValidateAntiForgeryToken] // من الجيد إضافة AntiForgeryToken هنا أيضاً
        public IActionResult Edit(Product model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _context.Products.Update(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                // التعامل مع حالات التزامن إذا كان المنتج قد تم تعديله أو حذفه من قبل شخص آخر
                if (!_context.Products.Any(e => e.Id == model.Id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "حدث خطأ أثناء تحديث المنتج.");
                return View(model);
            }
        }

        // GET: /Products/Delete/{id}
        [AuthorizePermission("Products_Delete")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: /Products/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [AuthorizePermission("Products_Delete")]
        [ValidateAntiForgeryToken] // من الجيد إضافة AntiForgeryToken هنا
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            
            // تحقق من وجود المنتج قبل الحذف لتجنب الاستثناء
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}