using Ban_Do_An_Vat.Data;
using Ban_Do_An_Vat.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Ban_Do_An_Vat.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly IGeminiService _geminiService;
        private readonly ApplicationDbContext _context;

        public ChatbotController(IGeminiService geminiService, ApplicationDbContext context)
        {
            _geminiService = geminiService;
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return Json(new { success = false, reply = "Vui lòng nhập tin nhắn." });
            }

            // Build system context with full product & combo data
            var systemContext = await BuildSystemContextAsync();

            var reply = await _geminiService.ChatAsync(request.Message, systemContext);
            return Json(new { success = true, reply });
        }

        private async Task<string> BuildSystemContextAsync()
        {
            var snacks = await _context.Snacks
                .Include(s => s.Category)
                .Where(s => s.IsAvailable)
                .OrderBy(s => s.CategoryId)
                .ToListAsync();

            var combos = await _context.Combos
                .Include(c => c.ComboItems)
                    .ThenInclude(ci => ci.Snack)
                .Where(c => c.IsAvailable)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Bạn là trợ lý AI của cửa hàng đồ ăn vặt MUNCHIES - một tiệm chuyên bán đồ ăn vặt Việt Nam chất lượng cao.");
            sb.AppendLine("NHIỆM VỤ: Chỉ trả lời các câu hỏi liên quan đến đồ ăn vặt, sản phẩm tại cửa hàng, combo ưu đãi, giá cả, nguyên liệu, hương vị, cách đặt hàng và các chủ đề ẩm thực.");
            sb.AppendLine("QUY TẮC BẮT BUỘC:");
            sb.AppendLine("- Nếu khách hỏi về chủ đề KHÔNG liên quan đến đồ ăn vặt (chính trị, thể thao, công nghệ, y tế, v.v.), hãy lịch sự từ chối và hướng dẫn họ hỏi về sản phẩm.");
            sb.AppendLine("- Luôn trả lời bằng tiếng Việt thân thiện, dễ hiểu.");
            sb.AppendLine("- Gợi ý sản phẩm phù hợp khi khách hỏi về sở thích.");
            sb.AppendLine("- Thông tin giá cả đã bao gồm VAT.");
            sb.AppendLine();
            sb.AppendLine("=== DANH SÁCH SẢN PHẨM ===");

            string? currentCategory = null;
            foreach (var snack in snacks)
            {
                if (snack.Category?.Name != currentCategory)
                {
                    currentCategory = snack.Category?.Name;
                    sb.AppendLine($"\n[{currentCategory}]");
                }
                sb.AppendLine($"- {snack.Name}: {snack.Price:N0}đ | Trọng lượng: {snack.Weight} | Đánh giá: {snack.Rating}/5 | Thành phần: {snack.Ingredients}");
                sb.AppendLine($"  Mô tả: {snack.Description}");
            }

            if (combos.Any())
            {
                sb.AppendLine();
                sb.AppendLine("=== COMBO ƯU ĐÃI ===");
                foreach (var combo in combos)
                {
                    sb.AppendLine($"\n🎁 {combo.Name}");
                    sb.AppendLine($"   Giá gốc: {combo.OriginalPrice:N0}đ → Giá combo: {combo.SalePrice:N0}đ (Tiết kiệm {combo.DiscountPercent}%)");
                    sb.AppendLine($"   Mô tả: {combo.Description}");
                    sb.AppendLine("   Bao gồm:");
                    foreach (var item in combo.ComboItems)
                    {
                        sb.AppendLine($"     • {item.Snack?.Name} x{item.Quantity}");
                    }
                }
            }
            else
            {
                sb.AppendLine("\n=== COMBO ƯU ĐÃI ===");
                sb.AppendLine("Hiện tại chưa có combo nào. Hãy tư vấn khách hàng mua sản phẩm đơn lẻ.");
            }

            return sb.ToString();
        }
    }

    public class ChatRequest
    {
        public string? Message { get; set; }
    }
}
