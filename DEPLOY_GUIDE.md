# Hướng dẫn Deploy Ban Đồ Ăn Vặt lên Render + Neon

> Tạo 07/08/2026 | ASP.NET Core 8 | PostgreSQL (Neon) | Docker (Render)

---

## Tổng quan kiến trúc

`
GitHub Repo
    ↓ (auto-deploy on push)
Render.com (Docker Web Service)
    ↓ (DATABASE_URL env var)
Neon.tech (PostgreSQL Database)
`

---

## Bước 1: Chuẩn bị GitHub Repository

### 1.1 Khởi tạo Git (nếu chưa có)
`ash
cd D:\Ban_Do_An_Vat
git init
git add .
git commit -m "Initial commit - Ban Do An Vat"
`

### 1.2 Tạo repository trên GitHub
1. Vào github.com → New repository
2. Đặt tên: Ban_Do_An_Vat (private hoặc public đều được)
3. KHÔNG tick "Add a README" (đã có code rồi)

### 1.3 Push code lên GitHub
`ash
git remote add origin https://github.com/TEN_BAN/Ban_Do_An_Vat.git
git branch -M main
git push -u origin main
`

---

## Bước 2: Cấu hình Neon Database

Connection string của bạn:
`
postgresql://neondb_owner:npg_3FORPdBsikN1@ep-blue-cell-axqvbg3f-pooler.c-4.us-east-2.aws.neon.tech/neondb?sslmode=require&channel_binding=require
`

> Database đã sẵn sàng! App sẽ tự động chạy migrations khi khởi động lần đầu trên Render.

---

## Bước 3: Tạo Web Service trên Render

1. Vào render.com → Dashboard → **New** → **Web Service**
2. Chọn **Connect a repository** → kết nối GitHub → chọn repo Ban_Do_An_Vat
3. Cấu hình:
   - **Name**: an-do-an-vat
   - **Region**: Singapore (gần VN nhất)
   - **Branch**: main
   - **Runtime**: Docker *(Render tự detect Dockerfile)*
   - **Plan**: Free

---

## Bước 4: Điền Environment Variables trên Render

Trong trang cấu hình service → tab **Environment** → thêm các biến sau:

| Key | Value |
|-----|-------|
| DATABASE_URL | postgresql://neondb_owner:npg_3FORPdBsikN1@ep-blue-cell-axqvbg3f-pooler.c-4.us-east-2.aws.neon.tech/neondb?sslmode=require&channel_binding=require |
| ASPNETCORE_ENVIRONMENT | Production |
| Gemini__ApiKey | *(Gemini API key của bạn)* |
| Momo__ReturnUrl | https://TEN_APP.onrender.com/Cart/MomoCallback |
| Momo__IpnUrl | https://TEN_APP.onrender.com/Cart/MomoIpn |

> **Lưu ý**: Thay TEN_APP bằng tên service thật sau khi tạo xong trên Render.

---

## Bước 5: Deploy

1. Nhấn **Create Web Service**
2. Render sẽ tự động:
   - Pull code từ GitHub
   - Build Docker image
   - Chạy container
   - App sẽ tự migrate database (InitialPostgres migration)
3. Sau 3-5 phút → app live tại https://TEN_APP.onrender.com

---

## Bước 6: Kiểm tra sau deploy

- [ ] Truy cập URL → trang chủ hiện đúng
- [ ] Đăng nhập admin: dmin@bandoanvat.com / Admin@123
- [ ] Kiểm tra dữ liệu seed (categories, snacks)
- [ ] Thử đặt hàng

---

## Lưu ý quan trọng

### Local Development (không thay đổi)
`ash
dotnet run
# Vẫn dùng SQL Server như bình thường
# appsettings.json → DefaultConnection → localdb
`

### Thêm migration mới khi phát triển
`ash
# Migration SQL Server (local) - như thường
dotnet ef migrations add TenMigration

# Migration PostgreSQL (Render) - folder riêng
="postgresql://..."; dotnet ef migrations add TenMigration --output-dir Data/MigrationsPostgres
`

### Free tier Render
- App sẽ ngủ sau 15 phút không có request
- Lần đầu truy cập sẽ chờ ~30 giây để wake up
- Muốn luôn online → upgrade lên Starter plan (/tháng)

---

## Troubleshooting

**Lỗi migration**: Kiểm tra Render logs → tab **Logs**

**Lỗi 500**: Đảm bảo DATABASE_URL đã được set đúng trong Render env vars

**App không nhận static files**: Đảm bảo wwwroot/ đã được push lên GitHub (không bị .gitignore)
