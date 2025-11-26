using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HealthApp.Services
{
    /// <summary>
    /// Service để gọi ChatGPT API
    /// </summary>
    public class ChatGPTService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";

        public ChatGPTService()
        {
            _apiKey = ConfigurationManager.AppSettings["ChatGPTApiKey"];
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30); // Timeout 30 giây
            if (!string.IsNullOrWhiteSpace(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("WARNING: ChatGPT API Key không được cấu hình trong App.config!");
            }
        }

        /// <summary>
        /// Gọi ChatGPT để đề xuất món ăn dựa trên mục tiêu
        /// </summary>
        /// <param name="loaiBuaAn">Loại bữa ăn (Sáng, Trưa, Tối)</param>
        /// <param name="mucTieu">Mục tiêu của user</param>
        /// <param name="danhSachMonAn">Danh sách món ăn có sẵn trong database</param>
        /// <param name="ngayDeXuat">Ngày đề xuất (để đa dạng hóa theo ngày)</param>
        /// <param name="monAnDaDeXuat">Danh sách món đã đề xuất ở các bữa trước (để tránh trùng lặp)</param>
        /// <returns>Danh sách tên món ăn được đề xuất</returns>
        public async Task<List<string>> SuggestFoodsAsync(string loaiBuaAn, string mucTieu, List<string> danhSachMonAn, DateTime? ngayDeXuat = null, List<string> monAnDaDeXuat = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_apiKey))
                {
                    System.Diagnostics.Debug.WriteLine("ChatGPT API Key không được cấu hình!");
                    return new List<string>();
                }

                // Tạo prompt cho ChatGPT
                string prompt = BuildPrompt(loaiBuaAn, mucTieu, danhSachMonAn, ngayDeXuat, monAnDaDeXuat);

                // Tạo request body
                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "Bạn là chuyên gia dinh dưỡng. QUAN TRỌNG: Chỉ đề xuất các món ăn có trong danh sách được cung cấp. KHÔNG được đề xuất món ăn ngoài danh sách. Tên món ăn phải CHÍNH XÁC với tên trong danh sách. Chỉ trả về tên món ăn, mỗi món một dòng, không có số thứ tự, không có ký tự đặc biệt." },
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 200,
                    temperature = 0.7
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gọi API
                System.Diagnostics.Debug.WriteLine($"Đang gọi ChatGPT API: {ApiUrl}");
                System.Diagnostics.Debug.WriteLine($"Prompt: {prompt.Substring(0, Math.Min(100, prompt.Length))}...");
                
                var response = await _httpClient.PostAsync(ApiUrl, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"ChatGPT API error: {response.StatusCode} - {errorContent}");
                    throw new Exception($"ChatGPT API error: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"ChatGPT API response: {responseContent.Substring(0, Math.Min(200, responseContent.Length))}...");
                
                var result = JsonConvert.DeserializeObject<ChatGPTResponse>(responseContent);

                // Parse kết quả
                if (result?.choices != null && result.choices.Length > 0)
                {
                    string suggestedText = result.choices[0].message.content;
                    return ParseSuggestedFoods(suggestedText, danhSachMonAn);
                }

                return new List<string>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi gọi ChatGPT API: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Tạo prompt cho ChatGPT
        /// </summary>
        private string BuildPrompt(string loaiBuaAn, string mucTieu, List<string> danhSachMonAn, DateTime? ngayDeXuat = null, List<string> monAnDaDeXuat = null)
        {
            var prompt = new StringBuilder();
            prompt.AppendLine($"Người dùng có mục tiêu: {mucTieu}");
            
            // Thêm thông tin ngày để đa dạng hóa
            if (ngayDeXuat.HasValue)
            {
                string thuTrongTuan = ngayDeXuat.Value.ToString("dddd", new System.Globalization.CultureInfo("vi-VN"));
                prompt.AppendLine($"Đề xuất các món ăn cho bữa {loaiBuaAn} ngày {ngayDeXuat.Value:dd/MM/yyyy} ({thuTrongTuan}) phù hợp với mục tiêu này.");
                prompt.AppendLine("LƯU Ý: Hãy đa dạng hóa món ăn so với các ngày khác để tránh nhàm chán.");
            }
            else
            {
                prompt.AppendLine($"Đề xuất các món ăn cho bữa {loaiBuaAn} phù hợp với mục tiêu này.");
            }
            
            prompt.AppendLine();
            prompt.AppendLine("QUAN TRỌNG: Danh sách món ăn có sẵn trong database (CHỈ được chọn từ danh sách này):");
            foreach (var monAn in danhSachMonAn)
            {
                prompt.AppendLine($"- {monAn}");
            }
            prompt.AppendLine();
            
            // Thêm thông tin về món đã đề xuất ở các bữa trước
            if (monAnDaDeXuat != null && monAnDaDeXuat.Count > 0)
            {
                prompt.AppendLine("⚠️ QUAN TRỌNG - TRÁNH TRÙNG LẶP:");
                prompt.AppendLine($"Các món ăn sau đã được đề xuất ở các bữa trước trong ngày (Sáng/Trưa):");
                foreach (var monDaDeXuat in monAnDaDeXuat)
                {
                    prompt.AppendLine($"  - {monDaDeXuat}");
                }
                prompt.AppendLine("KHÔNG được đề xuất lại các món trên. Hãy chọn món KHÁC từ danh sách.");
                prompt.AppendLine();
            }
            
            prompt.AppendLine("LƯU Ý QUAN TRỌNG:");
            prompt.AppendLine("- CHỈ được đề xuất các món ăn có trong danh sách trên");
            prompt.AppendLine("- KHÔNG được đề xuất món ăn không có trong danh sách");
            prompt.AppendLine("- Tên món ăn phải CHÍNH XÁC với tên trong danh sách");
            if (monAnDaDeXuat != null && monAnDaDeXuat.Count > 0)
            {
                prompt.AppendLine("- KHÔNG được đề xuất lại các món đã được đề xuất ở các bữa trước");
            }
            
            // Thêm hướng dẫn đặc biệt cho mục tiêu giảm cân
            if (mucTieu != null && (mucTieu.Contains("giảm cân") || mucTieu.Contains("Giảm cân") || mucTieu.Contains("giảm cân")))
            {
                prompt.AppendLine();
                prompt.AppendLine("Mục tiêu: GIẢM CÂN. Hãy chọn từ danh sách trên:");
                prompt.AppendLine("- Món ăn ít calo, giàu protein và chất xơ");
                prompt.AppendLine("- Ưu tiên rau xanh, thịt nạc, cá");
                prompt.AppendLine("- Tránh món nhiều carb và chất béo");
                prompt.AppendLine("- Tổng calo trong ngày nên khoảng 1200-1500 kcal");
                prompt.AppendLine($"- Bữa {loaiBuaAn} nên có khoảng {(loaiBuaAn == "Sáng" ? "350-450" : loaiBuaAn == "Trưa" ? "400-500" : "300-400")} kcal");
            }
            
            prompt.AppendLine();
            prompt.AppendLine("YÊU CẦU: Chọn CHÍNH XÁC các món ăn từ danh sách trên (không thêm, không bớt tên). Đề xuất 3-7 món ăn. Chỉ trả về tên món ăn, mỗi món một dòng, không có số thứ tự, không có ký tự đặc biệt.");

            return prompt.ToString();
        }

        /// <summary>
        /// Parse kết quả từ ChatGPT và match với danh sách món ăn có sẵn
        /// </summary>
        private List<string> ParseSuggestedFoods(string suggestedText, List<string> danhSachMonAn)
        {
            var result = new List<string>();
            
            if (string.IsNullOrWhiteSpace(suggestedText))
                return result;

            // Tách các dòng
            var lines = suggestedText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                string cleanedLine = line.Trim();
                
                // Loại bỏ số thứ tự, dấu gạch đầu dòng, etc.
                cleanedLine = System.Text.RegularExpressions.Regex.Replace(cleanedLine, @"^[\d\.\-\*]\s*", "");
                cleanedLine = cleanedLine.Trim();

                // Tìm món ăn khớp trong danh sách (fuzzy match - ưu tiên exact match trước)
                var matchedFood = danhSachMonAn.FirstOrDefault(monAn =>
                    monAn.Equals(cleanedLine, StringComparison.OrdinalIgnoreCase)) // Exact match trước
                    ?? danhSachMonAn.FirstOrDefault(monAn =>
                    cleanedLine.IndexOf(monAn, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    monAn.IndexOf(cleanedLine, StringComparison.OrdinalIgnoreCase) >= 0); // Fuzzy match

                if (matchedFood != null && !result.Contains(matchedFood))
                {
                    result.Add(matchedFood);
                }
            }

            return result;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    /// <summary>
    /// Response model từ ChatGPT API
    /// </summary>
    public class ChatGPTResponse
    {
        public Choice[] choices { get; set; }
    }

    public class Choice
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public string content { get; set; }
    }
}

