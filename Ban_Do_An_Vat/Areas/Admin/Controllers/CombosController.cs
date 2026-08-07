using Ban_Do_An_Vat.Data;
using Ban_Do_An_Vat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ban_Do_An_Vat.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CombosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CombosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Combos
        public IActionResult Index()
        {
            ViewData["ActivePage"] = "Combos";
            var combos = _context.Combos
                .Include(c => c.ComboItems)
                    .ThenInclude(ci => ci.Snack)
                .OrderByDescending(c => c.Id)
                .ToList();
            return View(combos);
        }

        // GET: Admin/Combos/Create
        public IActionResult Create()
        {
            ViewData["ActivePage"] = "Combos";
            ViewBag.Snacks = _context.Snacks
                .Include(s => s.Category)
                .Where(s => s.IsAvailable)
                .OrderBy(s => s.Name)
                .ToList();
            return View();
        }

        // POST: Admin/Combos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Combo combo, IFormFile? ImageFile, List<int> SnackIds, List<int> Quantities)
        {
            // Remove navigation property validation
            ModelState.Remove("ComboItems");

            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                        await ImageFile.CopyToAsync(fileStream);
                    combo.ImageUrl = "/uploads/" + fileName;
                }

                combo.CreatedAt = DateTime.UtcNow;
                _context.Combos.Add(combo);
                await _context.SaveChangesAsync();

                // Add combo items
                for (int i = 0; i < SnackIds.Count; i++)
                {
                    if (SnackIds[i] > 0)
                    {
                        var qty = (i < Quantities.Count && Quantities[i] > 0) ? Quantities[i] : 1;
                        _context.ComboItems.Add(new ComboItem
                        {
                            ComboId = combo.Id,
                            SnackId = SnackIds[i],
                            Quantity = qty
                        });
                    }
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "Tạo combo thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["ActivePage"] = "Combos";
            ViewBag.Snacks = _context.Snacks
                .Include(s => s.Category)
                .Where(s => s.IsAvailable)
                .OrderBy(s => s.Name)
                .ToList();
            return View(combo);
        }

        // GET: Admin/Combos/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var combo = _context.Combos
                .Include(c => c.ComboItems)
                    .ThenInclude(ci => ci.Snack)
                .FirstOrDefault(c => c.Id == id);

            if (combo == null) return NotFound();

            ViewData["ActivePage"] = "Combos";
            ViewBag.Snacks = _context.Snacks
                .Include(s => s.Category)
                .Where(s => s.IsAvailable)
                .OrderBy(s => s.Name)
                .ToList();
            return View(combo);
        }

        // POST: Admin/Combos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Combo combo, IFormFile? ImageFile, List<int> SnackIds, List<int> Quantities)
        {
            if (id != combo.Id) return NotFound();

            ModelState.Remove("ComboItems");

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                            await ImageFile.CopyToAsync(fileStream);
                        combo.ImageUrl = "/uploads/" + fileName;
                    }
                    else if (string.IsNullOrEmpty(combo.ImageUrl))
                    {
                        var original = await _context.Combos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                        combo.ImageUrl = original?.ImageUrl;
                    }

                    _context.Update(combo);

                    // Re-sync combo items: remove old, add new
                    var oldItems = _context.ComboItems.Where(ci => ci.ComboId == id).ToList();
                    _context.ComboItems.RemoveRange(oldItems);

                    for (int i = 0; i < SnackIds.Count; i++)
                    {
                        if (SnackIds[i] > 0)
                        {
                            var qty = (i < Quantities.Count && Quantities[i] > 0) ? Quantities[i] : 1;
                            _context.ComboItems.Add(new ComboItem
                            {
                                ComboId = id,
                                SnackId = SnackIds[i],
                                Quantity = qty
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật combo thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Combos.Any(c => c.Id == id)) return NotFound();
                    throw;
                }
            }

            ViewData["ActivePage"] = "Combos";
            ViewBag.Snacks = _context.Snacks
                .Include(s => s.Category)
                .Where(s => s.IsAvailable)
                .OrderBy(s => s.Name)
                .ToList();
            return View(combo);
        }

        // POST: Admin/Combos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var combo = _context.Combos.Find(id);
            if (combo != null)
            {
                _context.Combos.Remove(combo);
                _context.SaveChanges();
                TempData["Success"] = "Xóa combo thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
