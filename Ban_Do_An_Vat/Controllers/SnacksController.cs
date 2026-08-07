using System;
using System.Linq;
using Ban_Do_An_Vat.Data;
using Ban_Do_An_Vat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ban_Do_An_Vat.Controllers
{
    public class SnacksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public class CatalogViewModel
        {
            public IQueryable<Snack> Snacks { get; set; }
            public System.Collections.Generic.List<Category> Categories { get; set; }
            public int? SelectedCategoryId { get; set; }
            public string SearchString { get; set; }
            public decimal? MaxPrice { get; set; }
            public string SortOrder { get; set; }
        }

        public SnacksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Snacks
        public IActionResult Index(int? category, string searchString, string sortOrder, decimal? maxPrice)
        {
            var categories = _context.Categories.ToList();
            var snacksQuery = _context.Snacks.Include(s => s.Category).AsQueryable();

            // Filter by Category
            if (category.HasValue)
            {
                snacksQuery = snacksQuery.Where(s => s.CategoryId == category.Value);
            }

            // Filter by Search
            if (!string.IsNullOrEmpty(searchString))
            {
                snacksQuery = snacksQuery.Where(s => s.Name.Contains(searchString) || s.Description.Contains(searchString));
            }

            // Filter by Price
            if (maxPrice.HasValue)
            {
                snacksQuery = snacksQuery.Where(s => s.Price <= maxPrice.Value);
            }

            // Sorting
            ViewData["CurrentSort"] = sortOrder;
            ViewData["PriceSortParm"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            ViewData["RatingSortParm"] = sortOrder == "rating_desc" ? "rating_asc" : "rating_desc";

            switch (sortOrder)
            {
                case "price_asc":
                    snacksQuery = snacksQuery.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    snacksQuery = snacksQuery.OrderByDescending(s => s.Price);
                    break;
                case "rating_desc":
                    snacksQuery = snacksQuery.OrderByDescending(s => s.Rating);
                    break;
                case "rating_asc":
                    snacksQuery = snacksQuery.OrderBy(s => s.Rating);
                    break;
                default:
                    snacksQuery = snacksQuery.OrderByDescending(s => s.Id);
                    break;
            }

            var viewModel = new CatalogViewModel
            {
                Snacks = snacksQuery,
                Categories = categories,
                SelectedCategoryId = category,
                SearchString = searchString,
                MaxPrice = maxPrice,
                SortOrder = sortOrder
            };

            return View(viewModel);
        }

        // GET: Snacks/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var snack = _context.Snacks
                .Include(s => s.Category)
                .FirstOrDefault(m => m.Id == id);
            
            if (snack == null)
            {
                return NotFound();
            }

            // Query recommendations from the same category
            ViewBag.Recommendations = _context.Snacks
                .Include(s => s.Category)
                .Where(s => s.CategoryId == snack.CategoryId && s.Id != snack.Id)
                .Take(4)
                .ToList();

            return View(snack);
        }
    }
}
