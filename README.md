# 🍿 Munchies - Hệ Thống Website Thương Mại Điện Tử Bán Đồ Ăn Vặt

![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Render%2FSupabase-4169E1?style=for-the-badge&logo=postgresql)
![SQL Server](https://img.shields.io/badge/SQL_Server-LocalDB-CC292B?style=for-the-badge&logo=microsoftsqlserver)
![xUnit](https://img.shields.io/badge/Testing-xUnit%20%2B%20TDD-00599C?style=for-the-badge)

Website thương mại điện tử chuyên cung cấp các sản phẩm đồ ăn vặt chất lượng cao, thiết kế giao diện hiện đại theo phong cách **Warm & Friendly (Forest Green / Coral / Warm Bone)**, hỗ trợ mua sắm giỏ hàng, mã giảm giá, đặt combo tiết kiệm, thanh toán đa phương thức và trợ lý AI tư vấn món ăn 24/7.

---

## 🏛 1. Kiến Trúc & Công Nghệ Hệ Thống (Architecture & Tech Stack)

### 💻 Core Framework & Frontend
- **Backend Framework**: ASP.NET Core 8.0 MVC (Model-View-Controller).
- **Frontend Engine**: Razor Views (`.cshtml`), HTML5, JavaScript (ES6+).
- **Styling System**: TailwindCSS + Vanilla CSS tùy biến design tokens (`brand-forest`, `brand-coral`, `brand-matcha`, `brand-bone`, `brand-amber`).
- **Iconography**: Phosphor Icons.

---

### 🗄 Kiến Trúc Cơ Sở Dữ Liệu Kép (Dual-Database Architecture)
Hệ thống linh hoạt tự động chuyển đổi giữa 2 hệ quản trị cơ sở dữ liệu:
1. **Local Development**: **Microsoft SQL Server (LocalDB)**
   - Context: `ApplicationDbContext`
   - Migrations: `Ban_Do_An_Vat/Data/Migrations/`
   - Factory: `ApplicationDbContextFactory.cs` (hỗ trợ `dotnet ef` CLI ở design-time).
2. **Production Deployment (Render / Neon / Supabase)**: **PostgreSQL**
   - Context: `ApplicationDbContextPostgres` (kế thừa `ApplicationDbContext`)
   - Migrations: `Ban_Do_An_Vat/Data/MigrationsPostgres/`
   - Factory: `ApplicationDbContextPostgresFactory.cs`
   - **Tự động chuyển đổi**: Kiểm tra biến môi trường `DATABASE_URL`. Khi phát hiện `DATABASE_URL` (dạng URI `postgresql://...`), hệ thống tự động:
     - Chuyển đổi URI sang dạng Key-Value chuẩn Npgsql.
     - Bật `Npgsql.EnableLegacyTimestampBehavior` để tương thích timestamp từ form input.
     - Tự động thực thi `db.Database.Migrate()` khi ứng dụng khởi chạy trên Production để cập nhật schema Supabase/PostgreSQL.

---

### 🖼 Cơ Chế Lưu Trữ & Phục Vụ Ảnh Binary (Database Image Storage)
- **Không phụ thuộc API bên ngoài**: File ảnh upload được chuyển đổi thành `byte[]` và lưu trực tiếp vào các cột `ImageData` & `ImageContentType` trong các bảng `Snacks`, `Combos`, và `Categories`.
- **ImageController (`/Image/{Type}/{id}`)**:
  - Tự động phục vụ ảnh binary từ DB với đúng `Content-Type`.
  - Thiết lập HTTP Response Caching (`86400s` = 1 ngày) để tối ưu băng thông.
  - Hỗ trợ Fallback tự động: Nếu chưa có `ImageData` nhưng có `ImageUrl` sẵn, tự động `Redirect` về URL cũ.

---

### 🔒 Bảo Mật & Phân Quyền (Security & Hardening)
- **Authentication & Authorization**: ASP.NET Core Identity (Role-based Authorization: `User` & `Admin`).
- **Security Headers Middleware**: Tự động cấu hình `X-Frame-Options: DENY`, `X-Content-Type-Options: nosniff`, `Referrer-Policy`, `Permissions-Policy`.
- **Rate Limiting Middleware**: 
  - Đăng nhập (`/Account/Login`): Giới hạn tối đa 10 lần / 15 phút mỗi IP chống brute-force.
  - Chatbot AI (`/Chatbot/SendMessage`): Giới hạn tối đa 20 lượt / phút mỗi IP.
- **Upload Security**: Whitelist MIME types (`image/jpeg`, `image/png`, `image/webp`), giới hạn kích thước tối đa 5MB.
- **Anti-XSS & Anti-CSRF**: Tự động xác thực `[ValidateAntiForgeryToken]` trên tất cả POST actions.

---

### 💳 Thanh Toán & Trợ Lý AI
- **Phương Thức Thanh Toán**:
  - **COD**: Thanh toán khi nhận hàng.
  - **VietQR**: Tạo mã QR chuyển khoản ngân hàng động (tự động điền số tiền & nội dung).
  - **MoMo**: Tích hợp MoMo Payment Gateway API (Signature HMAC SHA256).
- **AI Chatbot**: Tích hợp Google Gemini API (`GeminiService`) đóng vai trò trợ lý tư vấn đồ ăn vặt Munchies, hỗ trợ trả lời câu hỏi và gợi ý món ăn theo sở thích khách hàng.

---

## 📁 2. Cấu Trúc Thư Mục Dự Án (Project Structure)

```text
Ban_Do_An_Vat/
├── Ban_Do_An_Vat/                      # Project chính (ASP.NET Core Web MVC)
│   ├── Areas/
│   │   └── Admin/                      # Quản trị viên (Dashboard, Quản lý sản phẩm, Combo, Danh mục, Đơn hàng, Mã giảm giá)
│   │       └── Controllers/
│   ├── Controllers/                    # Controllers phía người dùng (Home, Snacks, Combos, Cart, Account, Image, Chatbot)
│   ├── Data/                           # Database Contexts & Migrations
│   │   ├── Migrations/                 # Migrations cho SQL Server (Local)
│   │   ├── MigrationsPostgres/         # Migrations cho PostgreSQL (Supabase / Production)
│   │   ├── ApplicationDbContext.cs
│   │   ├── ApplicationDbContextPostgres.cs
│   │   ├── ApplicationDbContextFactory.cs
│   │   └── ApplicationDbContextPostgresFactory.cs
│   ├── Models/                         # Entity Models (Snack, Combo, Category, Order, OrderItem, Coupon, CartItem, ApplicationUser)
│   ├── Services/                       # External Services (GeminiService, MomoService)
│   ├── Views/                          # Razor Views (Cart, Home, Snacks, Account, Shared,...)
│   ├── wwwroot/                        # Static Assets (CSS, JS, Uploads)
│   ├── appsettings.json                # Configuration file
│   └── Program.cs                      # Entry point & Middleware configuration
│
├── Ban_Do_An_Vat.Tests/                # Project Kiểm thử tự động (xUnit Test Suite)
│   ├── CartItemTests.cs                # Test tính toán tổng tiền giỏ hàng
│   ├── CouponAndShippingTests.cs       # Test mã giảm giá & chính sách phí vận chuyển
│   └── ImageControllerTests.cs         # Test quy trình phục vụ ảnh binary từ DB
│
└── .agents/skills/                     # Bộ Kỹ Năng Agent Tự Động (Agent Skills Directory)
```

---

## 🛠 3. Bộ Kỹ Năng Agent (.agents/skills)

Dự án tích hợp hệ thống **Agent Skills** giúp quy trình phát triển, kiểm thử, refactor và bảo mật được chuẩn hóa:

- 🧪 **`test-driven-development`**: Định hướng phát triển theo TDD (Red-Green-Refactor), viết unit test trước khi triển khai tính năng.
- 🎨 **`frontend-ui-engineering` & `impeccable`**: Đảm bảo tiêu chuẩn thiết kế UI/UX cao cấp, giao diện nhất quán, responsive, hỗ trợ micro-animations và chống thiết kế dạng "AI slop".
- 🛡 **`security-and-hardening`**: Đánh giá và áp dụng các biện pháp bảo mật multi-layer (Rate limiting, Security headers, Input validation, Whitelisting).
- 🔍 **`code-review-and-quality`**: Đánh giá chất lượng mã nguồn đa chiều trước khi commit/merge.
- 🐛 **`debugging-and-error-recovery`**: Tìm nguyên nhân gốc rễ (Root cause analysis) dựa trên empirical log thay vì sửa lỗi triệu chứng.
- 📐 **`api-and-interface-design`**: Thiết kế hợp đồng API và ranh giới các module mạch lạc.
- 🚀 **`shipping-and-launch` & `ci-cd-and-automation`**: Chuẩn hóa quy trình đóng gói, kiểm tra trước khi deploy (pre-flight checks).

---

## 🔑 4. Ghi Chú Về API Key & Cấu Hình Credentials (API Key Notice)

> ⚠️ **LƯU Ý THÀNH THẬT VỀ API KEYS & BẢO MẬT CONFIGURATION**:
> 
> Trong quá trình phát triển nhanh dự án, do tác giả... **"lười" chưa bọc mã hóa / KMS / Secret Manager** cho các API Keys, nên các thông số kết nối API hiện tại (bao gồm **Google Gemini API Key**, **MoMo Partner Credentials**, **VietQR Bank Info**) đang được lưu trữ trực tiếp dưới dạng **plain text** trong tệp `appsettings.json` hoặc cấu hình mặc định.
>
> 📌 **Khuyến nghị**: Khi đưa dự án vào môi trường thương mại hoặc production thực tế, vui lòng:
> 1. Trích xuất các Key này ra biến môi trường (`Environment Variables`) hoặc `User Secrets` / `Key Vault`.
> 2. Thay thế bằng API Key & Thông tin đối tác cá nhân/doanh nghiệp của bạn.

---

## 🚀 5. Hướng Dẫn Cài Đặt & Chạy Dự Án (Getting Started)

### Yêu cầu tiên quyết
- **.NET 8.0 SDK** trở lên.
- **SQL Server / LocalDB** (cho môi trường Dev) hoặc **PostgreSQL / Supabase** (cho môi trường Prod).

### Các bước chạy dự án local

1. **Clone repository**:
   ```bash
   git clone https://github.com/DaiAiTienTon/Ban_Do_An_Vat.git
   cd Ban_Do_An_Vat
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Cập nhật Cơ sở dữ liệu (SQL Server Local)**:
   ```bash
   dotnet ef database update --project Ban_Do_An_Vat --context ApplicationDbContext
   ```

4. **Chạy Unit Test Suite**:
   ```bash
   dotnet test
   ```

5. **Khởi chạy ứng dụng**:
   ```bash
   dotnet run --project Ban_Do_An_Vat
   ```
   Ứng dụng sẽ chạy tại `https://localhost:7147` hoặc `http://localhost:5147`.

---

© 2026 Munchies Team. Built with ❤️ and .NET 8.0.
