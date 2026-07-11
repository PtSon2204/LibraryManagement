using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.AiDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LibraryManagement.Business.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string? _apiKey;
        private static readonly HttpClient _httpClient = new();

        public GeminiService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _apiKey = configuration["GeminiSettings:ApiKey"];
            
            // Fallback to Env if needed (DotNetEnv is loaded)
            if (string.IsNullOrEmpty(_apiKey))
            {
                _apiKey = Environment.GetEnvironmentVariable("GeminiSettings__ApiKey");
            }
        }

        public async Task<string> ChatWithLibraryContextAsync(string prompt, List<ChatMessageDto> history)
        {
            // 1. Thu thập bối cảnh từ Cơ sở dữ liệu
            int bookCount = 0;
            int categoryCount = 0;
            int roomCount = 0;
            List<string> categoryNames = new();
            List<string> latestBooks = new();

            try
            {
                bookCount = await _unitOfWork.Books.Query().CountAsync();
                categoryCount = await _unitOfWork.Categories.Query().CountAsync();
                roomCount = await _unitOfWork.Rooms.Query().CountAsync();

                categoryNames = await _unitOfWork.Categories.Query()
                    .OrderBy(c => c.CategoryName)
                    .Select(c => c.CategoryName)
                    .Take(15)
                    .ToListAsync();

                latestBooks = await _unitOfWork.Books.Query()
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(5)
                    .Select(b => $"{b.Title} (ISBN: {b.ISBN})")
                    .ToListAsync();
            }
            catch (Exception)
            {
                // Bỏ qua nếu có lỗi kết nối DB khi khởi động, chạy ở chế độ cô lập
            }

            string categoryListStr = categoryNames.Any() ? string.Join(", ", categoryNames) : "Khoa học, CNTT, Văn học, Kinh tế, Ngoại ngữ";
            string bookListStr = latestBooks.Any() ? string.Join("\n- ", latestBooks) : "Lập trình C# cơ bản, Giáo trình OOD, Kỹ nghệ phần mềm";

            // 2. Kiểm tra API Key, nếu không cấu hình thì chạy chế độ Demo
            if (string.IsNullOrWhiteSpace(_apiKey) || _apiKey.Contains("YOUR_GEMINI_API_KEY"))
            {
                return GetDemoResponse(prompt, bookCount, categoryListStr, bookListStr, roomCount);
            }

            // 2.5. Lọc câu hỏi không liên quan (Topic Guard - Lớp 1: Server-side)
            var topicResult = CheckTopicRelevance(prompt, categoryNames);
            if (topicResult != null)
            {
                return topicResult;
            }

            // 3. Xây dựng System Instruction (Bối cảnh hệ thống cho AI)
            string systemInstruction = $@"Bạn là Trợ lý ảo AI thân thiện và chuyên nghiệp của Thư viện trường học.
Nhiệm vụ của bạn là hỗ trợ bạn đọc (học sinh, sinh viên, giáo viên) các thông tin liên quan đến thư viện.

Bối cảnh thư viện hiện tại:
- Tổng số lượng đầu sách trong hệ thống: {bookCount} cuốn.
- Số lượng thể loại sách: {categoryCount}. Các thể loại tiêu biểu gồm: {categoryListStr}.
- Số lượng phòng tự học/thảo luận: {roomCount} phòng.
- Một số sách mới cập nhật:
  - {bookListStr}

Hướng dẫn quy định mượn/trả sách của thư viện:
1. Mỗi bạn đọc được mượn tối đa 3 cuốn sách trong thời gian tối đa 14 ngày.
2. Bạn đọc có thể gia hạn thêm 1 lần (tối đa 7 ngày) trên trang quản lý tài khoản nếu sách chưa bị trễ hạn hoặc có người khác đặt trước.
3. Nếu trả sách trễ hạn, mức phạt sẽ tính theo quy định (thường là 5,000 VND / ngày trễ hạn).
4. Bạn đọc có thể đặt trước (Reserve) sách đang bận trên hệ thống web.

Hướng dẫn đặt phòng tự học/thảo luận:
1. Người dùng (Reader) có thể đặt phòng bằng cách vào mục 'Phòng thư viện' -> 'Đặt phòng'.
2. Mỗi phòng thảo luận hỗ trợ các slot thời gian chuẩn (khung giờ). Người dùng chọn phòng, tầng và slot trống để đặt.
3. Nếu quá 15 phút từ khi slot bắt đầu mà người dùng không Check-in tại quầy thủ thư, slot đặt phòng sẽ tự động bị hủy.

Nguyên tắc trả lời (BẮT BUỘC tuân thủ nghiêm ngặt):
- Trả lời bằng tiếng Việt, lịch sự, ngắn gọn, dùng Markdown (in đậm, danh sách, xuống dòng).
- CHỈ trả lời các câu hỏi liên quan đến: thư viện, sách, mượn/trả sách, đặt phòng, thể loại sách, tác giả, quy định thư viện, tài khoản bạn đọc.
- Nếu câu hỏi KHÔNG liên quan đến các chủ đề trên (ví dụ: thời tiết, nấu ăn, tình cảm, game, chính trị, lập trình không liên quan học tập...), bạn PHẢI từ chối bằng cách trả lời đúng mẫu sau:
  '🚫 Xin lỗi, tôi chỉ có thể hỗ trợ các câu hỏi liên quan đến **thư viện**. Bạn có thể hỏi tôi về tìm sách, mượn/trả sách, đặt phòng học, hoặc các quy định của thư viện nhé!'
- KHÔNG bao giờ đưa ra lời khuyên y tế, pháp lý, tài chính cá nhân, hoặc tạo nội dung sáng tạo không liên quan thư viện.
- Nếu người dùng muốn tìm sách cụ thể, hướng dẫn họ vào thanh Tìm kiếm hoặc mục 'Mượn sách'.
- Nếu người dùng muốn kiểm tra lịch sử mượn trả, hướng dẫn vào 'Hồ sơ cá nhân' -> 'Lịch sử mượn sách'.";

            // 4. Chuẩn bị Payload gửi tới Gemini API
            try
            {
                var contentsNode = new JsonArray();

                // Đưa lịch sử chat vào contents (đảm bảo đúng cấu trúc role: user/model và parts)
                foreach (var msg in history)
                {
                    string mappedRole = msg.Role.ToLower() == "user" ? "user" : "model";
                    contentsNode.Add(new JsonObject
                    {
                        ["role"] = mappedRole,
                        ["parts"] = new JsonArray
                        {
                            new JsonObject { ["text"] = msg.Content }
                        }
                    });
                }

                // Đưa prompt hiện tại vào cuối lịch sử
                contentsNode.Add(new JsonObject
                {
                    ["role"] = "user",
                    ["parts"] = new JsonArray
                    {
                        new JsonObject { ["text"] = prompt }
                    }
                });

                var payload = new JsonObject
                {
                    ["contents"] = contentsNode,
                    ["systemInstruction"] = new JsonObject
                    {
                        ["parts"] = new JsonArray
                        {
                            new JsonObject { ["text"] = systemInstruction }
                        }
                    }
                };

                // Thử lần lượt các model để tìm model có quota khả dụng
                string[] models = {
                    "gemini-3-flash-preview",
                    "gemini-2.0-flash-exp",
                    "gemini-1.5-flash-latest",
                    "gemini-1.5-flash",
                    "gemini-2.0-flash-lite"
                };
                HttpResponseMessage? response = null;
                bool success = false;

                foreach (var model in models)
                {
                    string url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_apiKey}";
                    var content = new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync(url, content);

                    // Nếu thành công hoặc lỗi không phải quota thì dừng vòng lặp
                    if (response.IsSuccessStatusCode || (int)response.StatusCode != 429)
                    {
                        success = true;
                        break;
                    }
                }

                if (response == null) return GetDemoResponse(prompt, bookCount, categoryListStr, bookListStr, roomCount,
                    overrideHeader: "⚠️ **Trợ lý AI tạm thời không khả dụng**. Vui lòng thử lại sau!\n\n");
                if (!response!.IsSuccessStatusCode)
                {
                    int statusCode = (int)response.StatusCode;

                    // 429: Hết quota → fallback về Demo
                    if (statusCode == 429)
                    {
                        return GetDemoResponse(prompt, bookCount, categoryListStr, bookListStr, roomCount,
                            overrideHeader: "⚠️ **Trợ lý AI tạm thời không khả dụng** do vượt giới hạn quota miễn phí của Gemini API. Đang sử dụng phản hồi mẫu:\n\n");
                    }

                    // 404: Sai tên model
                    if (statusCode == 404)
                    {
                        return "❌ Model AI không tìm thấy. Vui lòng liên hệ quản trị viên để cập nhật cấu hình.";
                    }

                    // 401/403: Sai key
                    if (statusCode == 401 || statusCode == 403)
                    {
                        return "🔑 API Key không hợp lệ hoặc đã hết hạn. Vui lòng liên hệ quản trị viên để cập nhật key Gemini.";
                    }

                    string errorMsg = await response.Content.ReadAsStringAsync();
                    return $"⚠️ Dịch vụ AI tạm thời gặp sự cố (mã lỗi: {statusCode}). Vui lòng thử lại sau ít phút!";
                }

                string responseJson = await response.Content.ReadAsStringAsync();
                var responseNode = JsonNode.Parse(responseJson);
                
                string aiText = responseNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.GetValue<string>()
                                ?? "Rất tiếc, tôi không thể xử lý câu trả lời lúc này.";

                return aiText;
            }
            catch (Exception ex)
            {
                return $"⚠️ Đã xảy ra lỗi kết nối với AI. Vui lòng thử lại sau!";
            }
        }

        private string GetDemoResponse(string prompt, int bookCount, string categories, string latestBooks, int roomCount, string? overrideHeader = null)
        {
            string cleanPrompt = prompt.Trim().ToLower();

            string demoHeader = overrideHeader 
                ?? "> [!NOTE]\n> **Hệ thống đang hoạt động ở chế độ Demo (Offline)**. Vui lòng thêm `GeminiSettings__ApiKey` trong tệp `.env` để kết nối với Trợ lý AI thực tế.\n\n";

            if (cleanPrompt.Contains("sách") || cleanPrompt.Contains("thể loại") || cleanPrompt.Contains("tìm"))
            {
                return demoHeader + 
                       $"Thư viện hiện tại có **{bookCount} cuốn sách** thuộc nhiều thể loại khác nhau.\n\n" +
                       $"**Các thể loại tiêu biểu:** {categories}.\n\n" +
                       $"**Một số sách mới cập nhật:**\n- {latestBooks.Replace("\n", "\n")}\n\n" +
                       $"Bạn có thể tìm kiếm sách trực tiếp trên trang chủ bằng cách sử dụng thanh tìm kiếm ở đầu trang hoặc vào mục **Mượn sách**.";
            }

            if (cleanPrompt.Contains("phòng") || cleanPrompt.Contains("đặt") || cleanPrompt.Contains("slot"))
            {
                return demoHeader +
                       $"Thư viện hiện có **{roomCount} phòng tự học/thảo luận** phục vụ nhóm từ 4-10 người.\n\n" +
                       $"**Hướng dẫn đặt phòng:**\n" +
                       $"1. Đăng nhập với tài khoản sinh viên/bạn đọc.\n" +
                       $"2. Trên menu chọn **Phòng thư viện** -> **Đặt phòng**.\n" +
                       $"3. Chọn phòng trống và khung giờ (slot) mong muốn, sau đó nhấn xác nhận.\n" +
                       $"4. Hãy đến nhận phòng và check-in với thủ thư trong vòng **15 phút** kể từ thời điểm slot bắt đầu để tránh bị hủy đặt phòng tự động.";
            }

            if (cleanPrompt.Contains("quy định") || cleanPrompt.Contains("phạt") || cleanPrompt.Contains("mượn") || cleanPrompt.Contains("hạn"))
            {
                return demoHeader +
                       $"**Quy định mượn/trả sách của Thư viện:**\n\n" +
                       $"- **Hạn mức:** Bạn được mượn tối đa **3 cuốn sách** cùng lúc.\n" +
                       $"- **Thời gian:** Thời gian mượn tối đa là **14 ngày**.\n" +
                       $"- **Gia hạn:** Được phép gia hạn thêm 1 lần (tối đa **7 ngày**) trước khi hết hạn sách.\n" +
                       $"- **Trễ hạn:** Phạt **5,000 VND / ngày** cho mỗi cuốn sách trả muộn.\n\n" +
                       $"Bạn có thể tra cứu thông tin chi tiết về các phiếu mượn và khoản phạt trong trang **Hồ sơ cá nhân** của mình.";
            }

            return demoHeader +
                   $"Xin chào! Tôi là **Trợ lý AI của Thư viện** (Demo Mode).\n\n" +
                   $"Tôi có thể hỗ trợ bạn các chủ đề sau:\n" +
                   $"- 📚 **Tìm kiếm sách & thể loại sách** (hỏi về danh mục sách)\n" +
                   $"- 🚪 **Đặt phòng thảo luận** (hỏi về cách đặt slot, check-in)\n" +
                   $"- 📜 **Quy định mượn trả & tính phạt trễ hạn**\n\n" +
                   $"Hãy đặt câu hỏi cho tôi về bất kỳ chủ đề nào ở trên!";
        }
        

        /// <summary>
        /// Lớp 1 bảo vệ: Kiểm tra chủ đề câu hỏi trước khi gọi Gemini API.
        /// Trả về null nếu câu hỏi hợp lệ, trả về thông báo lỗi nếu không liên quan.
        /// </summary>
        private string? CheckTopicRelevance(string prompt, List<string> dbCategories)
        {
            string clean = prompt.Trim().ToLower();

            // Danh sách từ khóa LIÊN QUAN cố định → cho phép
            var allowedKeywords = new[]
            {
                // Sách & thư viện
                "sách", "book", "thư viện", "library", "isbn", "tựa đề", "tác giả", "author",
                "thể loại", "category", "genre", "nhà xuất bản", "publisher", "tìm", "search",
                // Mượn trả
                "mượn", "trả", "borrow", "return", "loan", "gia hạn", "renew", "hạn",
                "phiếu mượn", "lịch sử", "đang mượn",
                // Đặt phòng
                "phòng", "room", "đặt phòng", "reservation", "slot", "khung giờ", "check-in",
                "tầng", "floor", "thảo luận",
                // Quy định & tài khoản
                "quy định", "phạt", "fine", "tài khoản", "account", "đăng ký", "register",
                "hồ sơ", "profile", "mật khẩu", "password", "bạn đọc", "reader",
                // Chào hỏi & cảm ơn (cho phép)
                "xin chào", "hello", "hi", "chào", "cảm ơn", "thank", "giúp", "help",
                "hỏi", "hướng dẫn", "guide", "thông tin", "info", "có gì", "gì vậy",
                "được không", "làm sao", "như thế nào", "bao nhiêu", "khi nào"
            };

            // Kiểm tra từ khóa cố định
            if (allowedKeywords.Any(k => clean.Contains(k)))
                return null;

            // Tự động kiểm tra các thể loại sách thực tế trong DB
            // (ví dụ: "Fantasy", "Novel", "CNTT", "Văn học"... sẽ tự động được nhận dạng)
            if (dbCategories.Any(cat => clean.Contains(cat.ToLower())))
                return null;

            // Câu hỏi ngắn (<= 10 ký tự) → có thể là lời chào, cho phép
            if (clean.Length <= 10)
                return null;

            // Danh sách chủ đề RÕ RÀNG KHÔNG LIÊN QUAN → chặn ngay
            var blockedKeywords = new[]
            {
                "thời tiết", "weather", "nấu ăn", "công thức", "recipe", "tình yêu", "yêu em",
                "game", "minecraft", "liên minh", "lol ", "tiktok", "youtube", "netflix",
                "cổ phiếu", "bitcoin", "crypto", "tiền tệ", "forex",
                "chính trị", "bầu cử", "tổng thống", "chủ tịch",
                "bài tập về nhà", "giải hộ", "viết hộ", "làm hộ", "essay", "code hộ",
                "truyện cười", "joke", "thơ tình", "thơ văn",
                "bệnh", "thuốc", "chữa trị", "triệu chứng", "y tế", "bác sĩ",
                "luật sư", "pháp lý", "kiện", "toà án"
            };

            if (blockedKeywords.Any(k => clean.Contains(k)))
            {
                return "🚫 **Ngoài phạm vi hỗ trợ!**\n\n" +
                       "Tôi chỉ có thể hỗ trợ các câu hỏi liên quan đến **thư viện**. Bạn có thể hỏi tôi về:\n\n" +
                       "- 📚 Tìm sách, thể loại, tác giả\n" +
                       "- 📖 Mượn/trả sách, gia hạn, kiểm tra phiếu mượn\n" +
                       "- 🚪 Đặt phòng tự học, khung giờ slot\n" +
                       "- 📜 Quy định thư viện, mức phạt trễ hạn\n\n" +
                       "Bạn muốn hỏi về chủ đề nào?";
            }

            // Câu hỏi không rõ ràng nhưng không có từ khóa thư viện
            // → Cho phép AI tự xử lý (System Instruction đã đủ mạnh)
            return null;
        }
    }
}
