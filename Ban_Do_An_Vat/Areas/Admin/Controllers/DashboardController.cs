using System;
using System.Linq;
using Ban_Do_An_Vat.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Ban_Do_An_Vat.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["ActivePage"] = "Dashboard";

            // Dashboard Metrics
            var totalOrders = _context.Orders.Count();
            var totalRevenue = _context.Orders
                .Where(o => o.Status != "Cancelled")
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;
            var lowStockCount = _context.Snacks.Count(s => s.StockQuantity <= 15);
            var outOfStockCount = _context.Snacks.Count(s => s.StockQuantity == 0);

            // Recent Orders list
            var recentOrders = _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToList();

            // Stock Warnings list
            var warningProducts = _context.Snacks
                .Include(s => s.Category)
                .Where(s => s.StockQuantity <= 15)
                .OrderBy(s => s.StockQuantity)
                .Take(5)
                .ToList();

            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.LowStockCount = lowStockCount;
            ViewBag.OutOfStockCount = outOfStockCount;
            ViewBag.RecentOrders = recentOrders;
            ViewBag.WarningProducts = warningProducts;

            return View();
        }
    }
}
