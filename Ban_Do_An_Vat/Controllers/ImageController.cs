using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ban_Do_An_Vat.Data;

namespace Ban_Do_An_Vat.Controllers
{
    // ImageController - Serve ảnh binary từ database
    // Thêm 17/08/2026: Lưu ảnh trong DB (SQL Server & PostgreSQL / Supabase)
    // Endpoints:
    //   GET /Image/Snack/{id}
    //   GET /Image/Combo/{id}
    //   GET /Image/Category/{id}
    public class ImageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ImageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /Image/Snack/{id}
        [HttpGet]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Snack(int id)
        {
            var snack = await _context.Snacks
                .AsNoTracking()
                .Select(s => new { s.Id, s.ImageData, s.ImageContentType, s.ImageUrl })
                .FirstOrDefaultAsync(s => s.Id == id);

            if (snack == null) return NotFound();

            if (snack.ImageData != null && snack.ImageData.Length > 0)
            {
                var contentType = snack.ImageContentType ?? "image/jpeg";
                return File(snack.ImageData, contentType);
            }

            if (!string.IsNullOrWhiteSpace(snack.ImageUrl))
                return Redirect(snack.ImageUrl);

            return NotFound();
        }

        // GET /Image/Combo/{id}
        [HttpGet]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Combo(int id)
        {
            var combo = await _context.Combos
                .AsNoTracking()
                .Select(c => new { c.Id, c.ImageData, c.ImageContentType, c.ImageUrl })
                .FirstOrDefaultAsync(c => c.Id == id);

            if (combo == null) return NotFound();

            if (combo.ImageData != null && combo.ImageData.Length > 0)
            {
                var contentType = combo.ImageContentType ?? "image/jpeg";
                return File(combo.ImageData, contentType);
            }

            if (!string.IsNullOrWhiteSpace(combo.ImageUrl))
                return Redirect(combo.ImageUrl);

            return NotFound();
        }

        // GET /Image/Category/{id}
        [HttpGet]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Category(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Select(c => new { c.Id, c.ImageData, c.ImageContentType, c.ImageUrl })
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            if (category.ImageData != null && category.ImageData.Length > 0)
            {
                var contentType = category.ImageContentType ?? "image/jpeg";
                return File(category.ImageData, contentType);
            }

            if (!string.IsNullOrWhiteSpace(category.ImageUrl))
                return Redirect(category.ImageUrl);

            return NotFound();
        }
    }
}
