using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ban_Do_An_Vat.Services
{
    public interface IGeminiService
    {
        Task<string> ChatAsync(string userMessage, string systemContext);
    }

    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<GeminiService> _logger;

        public GeminiService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
            _logger = logger;
        }

        public async Task<string> ChatAsync(string userMessage, string systemContext)
        {
            try
            {
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

                var requestBody = new
                {
                    system_instruction = new
                    {
                        parts = new[]
                        {
                            new { text = systemContext }
                        }
                    },
                    contents = new[]
                    {
                        new
                        {
                            role = "user",
                            parts = new[]
                            {
                                new { text = userMessage }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 1024
                    },
                    safetySettings = new[]
                    {
                        new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
                        new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_MEDIUM_AND_ABOVE" }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API error {Status}: {Body}", response.StatusCode, errorBody);
                    
                    // Fallback to offline context search if quota/API is exceeded
                    return GetOfflineFallbackResponse(userMessage, systemContext);
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseJson);

                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return text ?? GetOfflineFallbackResponse(userMessage, systemContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API");
                return GetOfflineFallbackResponse(userMessage, systemContext);
            }
        }

        private string GetOfflineFallbackResponse(string userMessage, string systemContext)
        {
            var msgLower = userMessage.ToLower();

            // 1. Phí ship / Giao hàng / Đặt hàng
            if (msgLower.Contains("ship") || msgLower.Contains("giao hàng") || msgLower.Contains("vận chuyển") || msgLower.Contains("phí") || msgLower.Contains("địa chỉ"))
            {
                return "Munchies miễn phí giao hàng toàn quốc cho mọi đơn hàng từ 150.000đ trở lên. Đối với đơn hàng dưới 150.000đ, phí ship đồng giá chỉ 20.000đ nhé! Bạn có thể đặt mua hàng trực tiếp trên website bằng cách chọn sản phẩm lẻ hoặc các Combo ưu đãi và ấn nút đặt hàng ạ. 🥰";
            }

            // 2. Combo khuyến mại
            if (msgLower.Contains("combo") || msgLower.Contains("khuyến mãi") || msgLower.Contains("giảm giá") || msgLower.Contains("ưu đãi") || msgLower.Contains("tiết kiệm"))
            {
                var lines = systemContext.Split('\n');
                var combosList = new List<string>();
                bool insideCombo = false;
                var currentCombo = "";

                foreach (var line in lines)
                {
                    if (line.Contains("=== COMBO ƯU ĐÃI ==="))
                    {
                        insideCombo = true;
                        continue;
                    }
                    if (insideCombo)
                    {
                        if (line.Trim().StartsWith("🎁"))
                        {
                            if (!string.IsNullOrEmpty(currentCombo)) combosList.Add(currentCombo);
                            currentCombo = line.Trim();
                        }
                        else if (!string.IsNullOrEmpty(currentCombo) && (line.Trim().StartsWith("Giá gốc") || line.Trim().StartsWith("Mô tả")))
                        {
                            currentCombo += "\n" + line.Trim();
                        }
                    }
                }
                if (!string.IsNullOrEmpty(currentCombo)) combosList.Add(currentCombo);

                if (combosList.Any())
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Hiện tại Munchies đang có các combo siêu ưu đãi cực hấp dẫn này nè bạn ơi: ");
                    foreach (var c in combosList)
                    {
                        sb.AppendLine(c);
                    }
                    sb.AppendLine("\nMua combo giúp bạn tiết kiệm từ 15% - 30% so với mua lẻ từng món đó ạ! Bạn nhanh tay thêm vào giỏ hàng nhé! 🍿");
                    return sb.ToString();
                }
                
                return "Dạ hiện tại cửa hàng đang cập nhật thêm các combo mới. Bạn vui lòng xem danh sách các món lẻ của Munchies nha! 😋";
            }

            // 3. Tìm sản phẩm cụ thể
            var matchedProducts = new List<string>();
            var allLines = systemContext.Split('\n');
            bool insideProducts = false;
            foreach (var line in allLines)
            {
                if (line.Contains("=== DANH SÁCH SẢN PHẨM ==="))
                {
                    insideProducts = true;
                    continue;
                }
                if (line.Contains("=== COMBO ƯU ĐÃI ==="))
                {
                    insideProducts = false;
                }
                if (insideProducts && line.Trim().StartsWith("-"))
                {
                    var parts = line.Substring(1).Split('|');
                    var name = parts[0].Trim();
                    var cleanName = name.ToLower();
                    if (msgLower.Contains(cleanName) || cleanName.Contains(msgLower) || 
                        (msgLower.Contains("bánh tráng") && cleanName.Contains("bánh tráng")) ||
                        (msgLower.Contains("khô") && cleanName.Contains("khô")) ||
                        (msgLower.Contains("cơm cháy") && cleanName.Contains("cơm cháy")) ||
                        (msgLower.Contains("rong biển") && cleanName.Contains("rong biển")) ||
                        (msgLower.Contains("hạt") && cleanName.Contains("hạt")))
                    {
                        matchedProducts.Add(line.Trim());
                    }
                }
            }

            if (matchedProducts.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine("Munchies tìm thấy sản phẩm ăn vặt bạn quan tâm đây ạ: ");
                foreach (var p in matchedProducts.Take(3))
                {
                    sb.AppendLine(p);
                }
                sb.AppendLine("\nTất cả sản phẩm đều sẵn có và chuẩn vị Việt. Bạn thêm vào giỏ hàng ngay nhé! 🤤");
                return sb.ToString();
            }

            // 4. Mặc định
            return "Dạ, Munchies AI xin chào bạn! 🍿 Bạn muốn tìm hiểu về món ăn vặt nào (như Bánh Tráng Trộn, Khô Bò Cay, Khô Gà Lá Chanh, Cơm Cháy Chà Bông, Rong Biển Cháy Tỏi...) hay thông tin Combo Ưu đãi, Phí ship của shop không ạ? Hãy cứ nhắn để mình hỗ trợ nhé! (Lưu ý: Hệ thống AI đang tạm thời hoạt động ở chế độ Offline/Offline Fallback do quá tải yêu cầu API). ✨";
        }
    }
}
