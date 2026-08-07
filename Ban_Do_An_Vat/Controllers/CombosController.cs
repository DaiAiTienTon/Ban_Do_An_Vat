using Ban_Do_An_Vat.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ban_Do_An_Vat.Controllers
{
    public class CombosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CombosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Combos
        public IActionResult Index()
        {
            var combos = _context.Combos
                .Include(c => c.ComboItems)
                    .ThenInclude(ci => ci.Snack)
                        .ThenInclude(s => s!.Category)
                .Where(c => c.IsAvailable)
                .ToList()  // load vào bộ nhớ trước
                .OrderByDescending(c => c.DiscountPercent)  // sort in-memory (DiscountPercent là NotMapped)
                .ToList();

            return View(combos);
        }

        // GET: /Combos/Details/5
        public IActionResult Details(int id)
        {
            var combo = _context.Combos
                .Include(c => c.ComboItems)
                    .ThenInclude(ci => ci.Snack)
                        .ThenInclude(s => s!.Category)
                .FirstOrDefault(c => c.Id == id && c.IsAvailable);

            if (combo == null) return NotFound();

            return View(combo);
        }
    }
}
