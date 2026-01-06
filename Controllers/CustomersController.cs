using Microsoft.AspNetCore.Mvc;
using AutoPartsPOS.Data;
using AutoPartsPOS.Models;
using AutoPartsPOS.Filters;
using System.Linq;
using Microsoft.EntityFrameworkCore; // تمت الإضافة لجلب البيانات بشكل أفضل في المستقبل

namespace AutoPartsPOS.Controllers
{
    
[AuthorizeLogin]
public class CustomersController : Controller
{
    private readonly AppDbContext _context;
    public CustomersController(AppDbContext context) => _context = context;

    [AuthorizePermission("Customers_Read")]
    public IActionResult Index()
        => View(_context.Customers
        .Where(c => c.Name != "عميل نقدي")
        .OrderBy(c => c.Name)
        .ToList());

    [AuthorizePermission("Customers_Create")]
    public IActionResult Create() => View();

    [HttpPost]
    [AuthorizePermission("Customers_Create")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Customer model)
    {
        if (!ModelState.IsValid) 
        
        
        return View(model);

        _context.Customers.Add(model);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [AuthorizePermission("Customers_Update")]
    public IActionResult Edit(int id)
        => View(_context.Customers.Find(id));

    [HttpPost]
    [AuthorizePermission("Customers_Update")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Customer model)
    {
        if (!ModelState.IsValid) return View(model);

        _context.Customers.Update(model);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [AuthorizePermission("Customers_Delete")]
    public IActionResult Delete(int id)
        => View(_context.Customers.Find(id));

    [HttpPost, ActionName("Delete")]
    [AuthorizePermission("Customers_Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var customer = _context.Customers.Find(id);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}




}