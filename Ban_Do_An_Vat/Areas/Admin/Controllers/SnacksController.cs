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
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                    snack.ImageUrl = "/uploads/" + fileName;
                }

                _context.Snacks.Add(snack);
                _context.SaveChanges();
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
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }
                        snack.ImageUrl = "/uploads/" + fileName;
                    }
                    else
                    {
                        // Keep old ImageUrl if empty
                        if (string.IsNullOrEmpty(snack.ImageUrl))
                        {
                            var originalSnack = _context.Snacks.AsNoTracking().FirstOrDefault(s => s.Id == id);
                            if (originalSnack != null)
                            {
                                snack.ImageUrl = originalSnack.ImageUrl;
                            }
                        }
                    }

                    _context.Update(snack);
                    _context.SaveChanges();
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
