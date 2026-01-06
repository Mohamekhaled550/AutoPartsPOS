using AutoPartsPOS.Data;
using AutoPartsPOS.Models;
using AutoPartsPOS.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

[AuthorizeLogin]
public class SuppliersController : Controller
{
    private readonly AppDbContext _context;
    public SuppliersController(AppDbContext context) => _context = context;

    // ===================== INDEX =====================
    [AuthorizePermission("Suppliers_Read")]
    public IActionResult Index()
    {
        var suppliers = _context.Suppliers.ToList();
        return View(suppliers);
    }

    // ===================== CREATE =====================
    [AuthorizePermission("Suppliers_Create")]
    public IActionResult Create() => View();

    [HttpPost]
    [AuthorizePermission("Suppliers_Create")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Supplier model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _context.Suppliers.Add(model);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    // ===================== EDIT =====================
    [AuthorizePermission("Suppliers_Update")]
    public IActionResult Edit(int id)
    {
        var supplier = _context.Suppliers.Find(id);
        if (supplier == null) return NotFound();
        return View(supplier);
    }

    [HttpPost]
    [AuthorizePermission("Suppliers_Update")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Supplier model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _context.Suppliers.Update(model);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    // ===================== DELETE =====================
    [AuthorizePermission("Suppliers_Delete")]
    public IActionResult Delete(int id)
    {
        var supplier = _context.Suppliers.Find(id);
        if (supplier == null) return NotFound();
        return View(supplier);
    }

    [HttpPost, ActionName("Delete")]
    [AuthorizePermission("Suppliers_Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var supplier = _context.Suppliers.Find(id);
        if (supplier != null)
        {
            _context.Suppliers.Remove(supplier);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}
