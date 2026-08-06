# 📚 Library Management System

**Library Management** là nền tảng quản lý thư viện full-stack được xây dựng với **ASP.NET Core 8**, thiết kế theo kiến trúc **N-Layer** tách biệt rõ ràng giữa API backend, MVC frontend, Business Logic và Data Access. Hệ thống phục vụ đa vai trò: Admin, Thủ thư (Librarian) và Độc giả (Reader) với quy trình mượn trả sách, quản lý phòng đọc, phạt vi phạm và phân tích báo cáo toàn diện.

---

## ✨ Điểm nổi bật

- **Kiến trúc N-Layer**: Tách biệt hoàn toàn API, Business, Data và Models — dễ bảo trì và mở rộng.
- **Dual frontend**: REST API backend + ASP.NET MVC frontend giao tiếp qua HttpClient.
- **AI tích hợp**: Google Gemini AI hỗ trợ tra cứu và tư vấn sách thông minh.
- **Tự động hóa**: Background Service tự động quét và gửi email cảnh báo sách quá hạn.
- **OData support**: Truy vấn linh hoạt với `$filter`, `$select`, `$orderby`, `$expand` qua OData.
- **Thanh toán thực**: Tích hợp SePay webhook xử lý thanh toán tiền phạt.

---

## 🌐 Tính năng chính

| Tính năng | Chi tiết |
|---|---|
| 📖 **Quản lý sách** | Thêm/sửa/xóa sách, tác giả, thể loại, nhà xuất bản; upload ảnh bìa |
| 📋 **Quản lý bản sao** | Theo dõi từng bản sao sách theo vị trí kệ/tầng, trạng thái Available/Borrowed/Reserved |
| 🏷️ **Kệ & Tầng** | Quản lý Bookshelf, Shelf, ShelfSlot theo sơ đồ tầng thư viện |
| 🔄 **Mượn & Trả** | Quy trình mượn-trả đầy đủ, theo dõi trạng thái, lịch sử mượn trả |
| 📅 **Đặt trước** | Đặt trước sách, quản lý Reservation với hàng đợi ưu tiên |
| 🏠 **Phòng đọc** | Đặt phòng đọc theo khung giờ (Slot), khóa slot theo thời gian thực |
| 💰 **Phạt vi phạm** | Tính phạt tự động theo mẫu FineTemplate, thanh toán qua SePay |
| 📧 **Email tự động** | Cảnh báo sắp đến hạn, thông báo quá hạn gửi tự động qua MailKit |
| 🤖 **AI Chatbot** | Google Gemini AI tích hợp hỗ trợ tra cứu sách thông minh |
| 📊 **Dashboard & Báo cáo** | Thống kê lượt mượn, doanh thu phạt, sách phổ biến theo vai trò |
| 🔐 **Xác thực** | JWT Bearer Token (API), Cookie Auth + Google OAuth2 (MVC), phân quyền theo role |

---

## 🛠️ Tech Stack

| Layer | Công nghệ |
|---|---|
| **Backend API** | C# / ASP.NET Core 8 Web API |
| **Frontend** | ASP.NET Core 8 MVC, Razor Views, HTML/CSS/JS |
| **Database** | Microsoft SQL Server, Entity Framework Core 8 |
| **ORM** | EF Core (Code-First + Migrations) |
| **Auth** | JWT Bearer (API), Cookie Auth + Google OAuth2 (MVC) |
| **AI** | Google Gemini API |
| **Email** | MailKit / MimeKit (Gmail SMTP) |
| **Thanh toán** | SePay Webhook |
| **Query** | OData (Microsoft.AspNetCore.OData) |
| **Validation** | FluentValidation |
| **Config** | DotNetEnv (.env file) |

---

## 🏗️ Cấu trúc dự án

```
LibraryManagement/
├── LibraryManagement.API/          # REST API Backend
│   ├── Controllers/                # API Endpoints (Books, Loans, Fines, Rooms, ...)
│   ├── BackgroundServices/         # OverdueNotification Background Service
│   ├── Middleware/                 # Exception Middleware
│   ├── Extensions/                 # Service Registration Extensions
│   └── Program.cs                  # Entry point, OData config, DI setup
│
├── LibraryManagement.MVC/          # ASP.NET MVC Frontend
│   ├── Controllers/                # MVC Controllers (Admin, Librarian, Reader, ...)
│   ├── Views/                      # Razor Views (Book, Loan, Room, Shelf, ...)
│   ├── Services/                   # HttpClient Services gọi sang API
│   ├── ViewModels/                 # ViewModel cho từng trang
│   ├── Handlers/                   # Auth Handlers
│   └── wwwroot/                    # Static files (CSS, JS, Images)
│
├── LibraryManagement.Business/     # Business Logic Layer
│   ├── Services/                   # AuthService, BookService, LoanService, FineService, ...
│   ├── DTOs/                       # Data Transfer Objects
│   ├── Interfaces/                 # Service Interfaces
│   ├── Validators/                 # FluentValidation Validators
│   ├── AI/                         # Gemini AI Integration
│   └── Helpers/                    # Utility Helpers
│
├── LibraryManagement.Data/         # Data Access Layer
│   ├── Context/                    # ApplicationDbContext (EF Core)
│   ├── Repositories/               # Repository Pattern
│   ├── UnitOfWorks/                # Unit of Work
│   ├── Migrations/                 # EF Core Migrations
│   └── Interfaces/                 # Repository Interfaces
│
├── LibraryManagement.Models/       # Domain Models & Entities
│   ├── Models/                     # Entities: Book, Loan, Fine, Room, Shelf, ...
│   ├── Common/                     # Shared enums, constants
│   └── Queries/                    # Query parameters
│
├── *.sql                           # Seed data scripts
└── LibraryManagement.sln
```

---

## ⚙️ Chạy dự án locally

### Yêu cầu
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- Microsoft SQL Server (hoặc SQL Server Express)
- Visual Studio 2022+ / VS Code

### 1. Clone repository

```bash
git clone https://github.com/PtSon2204/LibraryManagement.git
cd LibraryManagement
```

### 2. Cấu hình Database

Mở SQL Server và tạo database `LibraryManagement`, sau đó cập nhật connection string trong `appsettings.json` của từng project:

```json
{
  "ConnectionStrings": {
    "MyCnn": "Server=YOUR_SERVER;Database=LibraryManagement;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Cấu hình biến môi trường

Tạo file `.env` trong thư mục `LibraryManagement.API/`:

```env
# Email (Gmail SMTP)
EmailSettings__SenderEmail=your_email@gmail.com
EmailSettings__SenderPassword=your_app_password

# Google Gemini AI
GeminiSettings__ApiKey=your_gemini_api_key

# Google OAuth2 (MVC)
Google__ClientId=your_google_client_id
Google__ClientSecret=your_google_client_secret
```

> **Lưu ý**: Với Gmail, cần bật **2-Step Verification** và tạo **App Password** tại [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords).

### 4. Apply Migrations & Seed Data

```bash
dotnet ef database update --project LibraryManagement.Data --startup-project LibraryManagement.API
```

Hoặc chạy trực tiếp các file SQL seed data có sẵn:

| File | Nội dung |
|---|---|
| `SeedData_52Books.sql` | 52 cuốn sách mẫu |
| `SeedData_ReaderLoansFines.sql` | Độc giả, lượt mượn, tiền phạt |
| `Add_DueSoon_He181997.sql` | Sách sắp đến hạn |
| `Add_Overdue_Oniichan.sql` | Sách quá hạn |

### 5. Chạy ứng dụng

Khuyến nghị dùng **Multiple Startup Projects** trong Visual Studio (chuột phải vào Solution → Set Startup Projects).

Hoặc chạy thủ công:

```bash
# Terminal 1 — API Backend
cd LibraryManagement.API
dotnet run

# Terminal 2 — MVC Frontend
cd LibraryManagement.MVC
dotnet run
```

| Project | URL mặc định |
|---|---|
| API Swagger UI | `https://localhost:PORT/swagger` |
| MVC Frontend | `https://localhost:PORT` |

---

## 🔌 API Endpoints nổi bật

| Method | Endpoint | Mô tả |
|---|---|---|
| `GET` | `/odata/Books` | Truy vấn sách với OData ($filter, $select, $orderby) |
| `POST` | `/api/Auth/login` | Đăng nhập, nhận JWT token |
| `GET` | `/api/Loans` | Danh sách lượt mượn |
| `POST` | `/api/Loans` | Tạo phiếu mượn mới |
| `PUT` | `/api/Loans/{id}/return` | Xử lý trả sách |
| `GET` | `/api/Fines` | Danh sách tiền phạt |
| `POST` | `/api/sepay-webhook` | Webhook nhận thanh toán SePay |
| `POST` | `/api/Ai/chat` | Chat với Gemini AI |
| `GET` | `/api/Reservations` | Danh sách đặt trước sách |
| `GET` | `/api/Rooms` | Danh sách phòng đọc |
| `GET` | `/api/Dashboard` | Dữ liệu dashboard Admin |
| `GET` | `/api/StaffDashboard` | Dữ liệu dashboard Thủ thư |

---

## 👥 Nhóm phát triển

Dự án được thực hiện trong khuôn khổ môn học **PRN232** tại FPT University:

| Tên | GitHub |
|---|---|
| Phạm Thế Sơn | [@PtSon2204](https://github.com/PtSon2204) |
| Nguyễn Đức Dũng | [@NguyenDucDung](https://github.com/NguyenDucDung) |
| Nguyễn Phúc Lâm | [@NguyenPhucLam](https://github.com/NguyenPhucLam) |

---

## 📬 Liên hệ

📧 Email: [he181997phamtheson@gmail.com](mailto:he181997phamtheson@gmail.com)  
🐙 GitHub: [https://github.com/PtSon2204/LibraryManagement](https://github.com/PtSon2204/LibraryManagement)
