using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ban_Do_An_Vat.Data;
using Ban_Do_An_Vat.Models;

namespace Ban_Do_An_Vat.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CouponsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Coupons
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Coupons";
            var coupons = await _context.Coupons.OrderByDescending(c => c.ExpiryDate).ToListAsync();
            return View(coupons);
        }

        // GET: Admin/Coupons/Create
        public IActionResult Create()
        {
            ViewData["ActivePage"] = "Coupons";
            return View();
        }

        // POST: Admin/Coupons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,DiscountAmount,DiscountType,MinOrderAmount,ExpiryDate,IsActive")] Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                // Ensure code is uppercase
                coupon.Code = coupon.Code.ToUpper().Trim();
                
                // Check if code already exists
                if (await _context.Coupons.AnyAsync(c => c.Code == coupon.Code))
                {
                    ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại.");
                    ViewData["ActivePage"] = "Coupons";
                    return View(coupon);
                }

                _context.Add(coupon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivePage"] = "Coupons";
            return View(coupon);
        }

        // GET: Admin/Coupons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            ViewData["ActivePage"] = "Coupons";
            return View(coupon);
        }

        // POST: Admin/Coupons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,DiscountAmount,DiscountType,MinOrderAmount,ExpiryDate,IsActive")] Coupon coupon)
        {
            if (id != coupon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    coupon.Code = coupon.Code.ToUpper().Trim();
                    
                    // Check if code already exists for other coupons
                    if (await _context.Coupons.AnyAsync(c => c.Code == coupon.Code && c.Id != id))
                    {
                        ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại.");
                        ViewData["ActivePage"] = "Coupons";
                        return View(coupon);
                    }

                    _context.Update(coupon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponExists(coupon.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivePage"] = "Coupons";
            return View(coupon);
        }

        // GET: Admin/Coupons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }

            ViewData["ActivePage"] = "Coupons";
            return View(coupon);
        }

        // POST: Admin/Coupons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CouponExists(int id)
        {
            return _context.Coupons.Any(e => e.Id == id);
        }
    }
}
