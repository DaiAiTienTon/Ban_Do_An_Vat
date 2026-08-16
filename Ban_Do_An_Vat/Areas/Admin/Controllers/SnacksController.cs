using System.Linq;
using Ban_Do_An_Vat.Data;
using Ban_Do_An_Vat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Ban_Do_An_Vat.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SnacksController : Controller
    {
        private readonly ApplicationDbContext _context;

        // [SEC-06] Whitelist file upload: chỉ cho phép những đuôi file/MIME type an toàn
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "image/webp" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public SnacksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Snacks
        public IActionResult Index()
        {
            ViewData["ActivePage"] = "Snacks";
            var snacks = _context.Snacks.Include(s => s.Category).OrderByDescending(s => s.Id).ToList();
            return View(snacks);
        }

        // GET: Admin/Snacks/Create
        public IActionResult Create()
        {
            ViewData["ActivePage"] = "Snacks";
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Admin/Snacks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Snack snack, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // [SEC-06] Kiểm tra extension và MIME type
                    var ext = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();
                    if (!AllowedExtensions.Contains(ext) || !AllowedMimeTypes.Contains(ImageFile.ContentType))
                    {
                        ModelState.AddModelError("ImageFile", "Chỉ chấp nhận ảnh JPG, PNG hoặc WebP.");
                        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", snack.CategoryId);
                        return View(snack);
                    }
                    if (ImageFile.Length > MaxFileSizeBytes)
                    {
                        ModelState.AddModelError("ImageFile", "Ảnh tối đa 5MB.");
                        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", snack.CategoryId);
                        return View(snack);
                    }

                    using (var ms = new MemoryStream())
                    {
                        await ImageFile.CopyToAsync(ms);
                        snack.ImageData = ms.ToArray();
                    }
                    snack.ImageContentType = ImageFile.ContentType;

                    _context.Snacks.Add(snack);
                    await _context.SaveChangesAsync();

                    snack.ImageUrl = $"/Image/Snack/{snack.Id}";
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                _context.Snacks.Add(snack);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", snack.CategoryId);
            return View(snack);
        }

        // GET: Admin/Snacks/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var snack = _context.Snacks.Find(id);
            if (snack == null)
            {
                return NotFound();
            }
            ViewData["ActivePage"] = "Snacks";
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", snack.CategoryId);
            return View(snack);
        }

        // POST: Admin/Snacks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Snack snack, IFormFile? ImageFile)
        {
            if (id != snack.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // [SEC-06] Kiểm tra extension và MIME type
                        var ext = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();
                        if (!AllowedExtensions.Contains(ext) || !AllowedMimeTypes.Contains(ImageFile.ContentType))
                        {
                            ModelState.AddModelError("ImageFile", "Chỉ chấp nhận ảnh JPG, PNG hoặc WebP.");
                            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", snack.CategoryId);
                            return View(snack);
                        }
                        if (ImageFile.Length > MaxFileSizeBytes)
                        {
                            ModelState.AddModelError("ImageFile", "Ảnh tối đa 5MB.");
                            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", snack.CategoryId);
                            return View(snack);
                        }

                        using (var ms = new MemoryStream())
                        {
                            await ImageFile.CopyToAsync(ms);
                            snack.ImageData = ms.ToArray();
                        }
                        snack.ImageContentType = ImageFile.ContentType;
                        snack.ImageUrl = $"/Image/Snack/{snack.Id}";
                    }
                    else
                    {
                        var originalSnack = await _context.Snacks.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                        if (originalSnack != null)
                        {
                            snack.ImageData = originalSnack.ImageData;
                            snack.ImageContentType = originalSnack.ImageContentType;
                            if (string.IsNullOrEmpty(snack.ImageUrl))
                            {
                                snack.ImageUrl = originalSnack.ImageUrl;
                            }
                        }
                    }

                    _context.Update(snack);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Snacks.Any(e => e.Id == snack.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", snack.CategoryId);
            return View(snack);
        }

        // POST: Admin/Snacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var snack = _context.Snacks.Find(id);
            if (snack != null)
            {
                _context.Snacks.Remove(snack);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
