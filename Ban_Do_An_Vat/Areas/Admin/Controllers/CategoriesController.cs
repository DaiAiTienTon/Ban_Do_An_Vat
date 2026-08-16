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
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Categories";
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            ViewData["ActivePage"] = "Categories";
            return View();
        }

        // POST: Admin/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ImageUrl,Description")] Category category, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await ImageFile.CopyToAsync(ms);
                        category.ImageData = ms.ToArray();
                    }
                    category.ImageContentType = ImageFile.ContentType;
                }

                _context.Add(category);
                await _context.SaveChangesAsync();

                if (category.ImageData != null && category.ImageData.Length > 0)
                {
                    category.ImageUrl = $"/Image/Category/{category.Id}";
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivePage"] = "Categories";
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ActivePage"] = "Categories";
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImageUrl,Description")] Category category, IFormFile? ImageFile)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await ImageFile.CopyToAsync(ms);
                            category.ImageData = ms.ToArray();
                        }
                        category.ImageContentType = ImageFile.ContentType;
                        category.ImageUrl = $"/Image/Category/{category.Id}";
                    }
                    else
                    {
                        var original = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                        if (original != null)
                        {
                            category.ImageData = original.ImageData;
                            category.ImageContentType = original.ImageContentType;
                            if (string.IsNullOrEmpty(category.ImageUrl))
                            {
                                category.ImageUrl = original.ImageUrl;
                            }
                        }
                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            ViewData["ActivePage"] = "Categories";
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            ViewData["ActivePage"] = "Categories";
            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
