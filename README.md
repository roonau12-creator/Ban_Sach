# BanSach

> Hệ thống quản lý và bán sách trực tuyến — ASP.NET Core MVC + Entity Framework Core + PostgreSQL

BanSach là ứng dụng web quản lý và bán sách, xây dựng bằng C# trên nền tảng ASP.NET Core MVC theo kiến trúc Areas, sử dụng Entity Framework Core kết nối PostgreSQL và giao diện Bootstrap 5. Hệ thống xác thực dựa trên Session (không dùng ASP.NET Identity).

## Công nghệ

| Thành phần | Phiên bản |
| --- | --- |
| .NET | 10.0 (`net10.0`) |
| ASP.NET Core MVC | .NET 10 (minimal hosting) |
| Entity Framework Core | 10.0.12 |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.3 |
| Bootstrap | 5.3.3 |
| jQuery | 3.7.1 |
| jQuery Validation / Unobtrusive | 1.21.0 |

Database: PostgreSQL (mặc định `localhost:5432`, database `BanSach`).

## Yêu cầu

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- PostgreSQL đang chạy

## Cài đặt & chạy

```bash
# 1. Cài package
dotnet restore

# 2. Áp dụng migration (tạo DB nếu chưa có, seed dữ liệu mẫu tự động)
dotnet ef database update

# 3. Chạy ứng dụng
dotnet run
```

Ứng dụng khởi động tại:

- http://localhost:5126
- https://localhost:7217

> Khi khởi động, `Program.cs` tự động chạy `db.Database.Migrate()` và `DbSeeder.Seed(db)` — không cần seed thủ công.

Nếu `dotnet ef` chưa có, cài tool toàn cục:

```bash
dotnet tool install --global dotnet-ef
```

## Cấu hình

Chuỗi kết nối trong `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=BanSach;Username=postgres;Password=123"
}
```

> **Lưu ý bảo mật:** không commit mật khẩu thực vào source code. Khi triển khai nên dùng user-secrets hoặc biến môi trường:
>
> ```bash
> dotnet user-secrets init
> dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Password=..."
> ```

## Cấu trúc dự án

```
BanSach/
├── Areas/
│   ├── Admin/                        # Khu vực quản trị
│   │   ├── Controllers/
│   │   │   ├── ChuDeController.cs
│   │   │   ├── TacGiaController.cs
│   │   │   ├── NhomSachController.cs
│   │   │   ├── NhaXuatBanController.cs
│   │   │   ├── SachController.cs
│   │   │   ├── KhachHangController.cs
│   │   │   ├── DonHangController.cs
│   │   │   ├── DashboardController.cs
│   │   │   ├── ThongKeController.cs
│   │   │   └── HomeController.cs
│   │   └── Views/
│   │       ├── ChuDe/        (Index, Create, Edit, Details, Delete)
│   │       ├── TacGia/       (Index, Create, Edit, Details, Delete)
│   │       ├── NhomSach/     (Index, Create, Edit, Details, Delete)
│   │       ├── NhaXuatBan/   (Index, Create, Edit, Details, Delete)
│   │       ├── Sach/         (Index, Create, Edit, Details, Delete)
│   │       ├── KhachHang/    (Index, Details)
│   │       ├── DonHang/      (Index, Details)
│   │       ├── Dashboard/
│   │       ├── ThongKe/
│   │       └── Shared/
│   └── Customer/                     # Khu vực khách hàng
│       ├── Controllers/
│       │   ├── HomeController.cs
│       │   ├── SachController.cs
│       │   ├── GioHangController.cs
│       │   ├── DonHangController.cs
│       │   ├── TaiKhoanController.cs
│       │   ├── DiaChiController.cs
│       │   └── SoDienThoaiController.cs
│       └── Views/
│           ├── Home/         (Index, Privacy)
│           ├── Sach/
│           ├── GioHang/
│           ├── DonHang/
│           ├── TaiKhoan/     (DangNhap, DangKy, QuenMatKhau, ...)
│           ├── DiaChi/
│           ├── SoDienThoai/
│           └── Shared/
├── Data/
│   └── ApplicationDbContext.cs
├── Filters/
│   ├── KiemTraDangNhapAttribute.cs   # Kiểm tra session đăng nhập
│   └── KiemTraVaiTroAttribute.cs     # Kiểm tra vai trò (Admin/Customer)
├── Helpers/
│   ├── HangSo.cs                     # Hằng số vai trò, trạng thái, session keys
│   ├── SessionGioHang.cs             # Quản lý giỏ hàng qua session
│   ├── AnhBiaHelper.cs               # Upload/xử lý ảnh bìa sách
│   └── DbSeeder.cs                   # Seed dữ liệu mẫu khi khởi động
├── Migrations/
│   ├── 20260913024742_ChuDe.cs
│   └── 20260914005958_FullSchema.cs
├── Models/
│   ├── Sach.cs
│   ├── ChuDe.cs
│   ├── TacGia.cs
│   ├── SachTacGia.cs
│   ├── NhomSach.cs
│   ├── NhaXuatBan.cs
│   ├── KhachHang.cs
│   ├── TaiKhoan.cs
│   ├── DonHang.cs
│   ├── ChiTietDonHang.cs
│   ├── GiaoHang.cs
│   ├── DiaChi.cs
│   ├── SoDienThoai.cs
│   ├── GioHangItem.cs
│   ├── ErrorViewModel.cs
│   └── ViewModels/
│       ├── SachFormViewModel.cs
│       ├── HomeViewModel.cs
│       ├── DangNhapViewModel.cs
│       ├── DangKyViewModel.cs
│       ├── QuenMatKhauViewModel.cs
│       └── DatHangViewModel.cs
├── Views/Shared/
│   ├── _Layout.cshtml
│   ├── _ValidationScriptsPartial.cshtml
│   └── Error.cshtml
├── wwwroot/
│   ├── css/site.css
│   ├── js/site.js
│   ├── lib/                  (bootstrap, jquery, jquery-validation)
│   └── uploads/bia/          (ảnh bìa sách)
├── Program.cs
└── appsettings.json
```

### Định tuyến

`Program.cs` khai báo 2 route:

| Route | Pattern | Mô tả |
| --- | --- | --- |
| `MyAreas` | `{area:exists}/{controller=Home}/{action=Index}/{id?}` | Truy cập trực tiếp area: `/Admin/ChuDe` |
| `default` | `{controller}/{action}/{id?}` (mặc định `area = Customer`) | `/`, `/Home/Index` → khu vực Customer |

## Xác thực & phân quyền

Hệ thống dùng **Session** thay vì ASP.NET Identity:

- Đăng nhập lưu thông tin vào session (`MaTK`, `TenDangNhap`, `HoTen`, `VaiTro`, `MaKH`).
- `KiemTraDangNhapAttribute` — filter kiểm tra session, redirect sang `/Customer/TaiKhoan/DangNhap` nếu chưa đăng nhập.
- `KiemTraVaiTroAttribute` — filter kiểm tra vai trò (`Admin` / `Customer`).
- Vai trò được định nghĩa trong `Helpers/HangSo.cs` (`VaiTroNguoiDung.Admin`, `VaiTroNguoiDung.Customer`).
- `TaiKhoan.MatKhauHash` — mật khẩu được hash trước khi lưu.
- Hỗ trợ quên mật khẩu qua `ResetToken` + `ResetTokenHetHan`.

## Chức năng

### Admin (`/Admin/...`)

| Module | Đường dẫn | Chức năng |
| --- | --- | --- |
| Dashboard | `/Admin/Dashboard` | Tổng quan |
| Chủ đề | `/Admin/ChuDe` | CRUD chủ đề sách |
| Tác giả | `/Admin/TacGia` | CRUD tác giả |
| Nhóm sách | `/Admin/NhomSach` | CRUD nhóm/thể loại sách |
| Nhà xuất bản | `/Admin/NhaXuatBan` | CRUD nhà xuất bản |
| Sách | `/Admin/Sach` | CRUD sách, upload ảnh bìa, gán tác giả |
| Khách hàng | `/Admin/KhachHang` | Xem danh sách, chi tiết khách hàng |
| Đơn hàng | `/Admin/DonHang` | Xem & cập nhật trạng thái đơn hàng |
| Thống kê | `/Admin/ThongKe` | Báo cáo doanh thu, đơn hàng |

### Customer (`/` hoặc `/Customer/...`)

| Module | Đường dẫn | Chức năng |
| --- | --- | --- |
| Trang chủ | `/` | Danh sách sách nổi bật, sách mới |
| Chi tiết sách | `/Sach/Details/{id}` | Xem thông tin sách |
| Giỏ hàng | `/GioHang` | Thêm/xóa/cập nhật số lượng (Session) |
| Đặt hàng | `/DonHang/DatHang` | Đặt hàng, chọn địa chỉ & SĐT |
| Đơn hàng | `/DonHang` | Lịch sử & theo dõi đơn hàng |
| Tài khoản | `/TaiKhoan` | Đăng ký, đăng nhập, quên mật khẩu |
| Địa chỉ | `/DiaChi` | Quản lý địa chỉ giao hàng |
| Số điện thoại | `/SoDienThoai` | Quản lý số điện thoại |

### Trạng thái đơn hàng

```
Chờ xác nhận → Đã xác nhận → Đang giao → Đã giao
                    ↓
                 Đã hủy
```

### Phương thức thanh toán

- **COD** — thanh toán khi nhận hàng
- **Chuyển khoản** — thanh toán trước

## Các Models chính

| Model | Mô tả |
| --- | --- |
| `Sach` | Sách (tên, giá, tồn kho, ảnh bìa, mô tả, năm XB, sách mới, nổi bật) |
| `ChuDe` | Chủ đề sách |
| `TacGia` | Tác giả (quan hệ nhiều-nhiều với `Sach` qua `SachTacGia`) |
| `NhomSach` | Nhóm/thể loại sách |
| `NhaXuatBan` | Nhà xuất bản |
| `TaiKhoan` | Tài khoản đăng nhập (hash mật khẩu, vai trò, reset token) |
| `KhachHang` | Thông tin khách hàng |
| `DonHang` | Đơn hàng (trạng thái, tổng tiền, PTTT, địa chỉ giao) |
| `ChiTietDonHang` | Chi tiết từng sách trong đơn |
| `GiaoHang` | Thông tin giao hàng |
| `DiaChi` | Địa chỉ giao hàng của khách |
| `SoDienThoai` | Số điện thoại của khách |
| `GioHangItem` | Item giỏ hàng (lưu trong session) |

## Lệnh thường dùng

```bash
dotnet run                            # Chạy ứng dụng
dotnet build                          # Build
dotnet ef migrations add <Ten>        # Tạo migration mới
dotnet ef database update             # Áp dụng migration
dotnet ef database drop               # Xóa database
```

## Kiến trúc

- **Areas** tách khu vực Admin và Customer. Cả hai dùng chung `Views/Shared/_Layout.cshtml`.
- **Session-based auth** — `KiemTraDangNhapAttribute` và `KiemTraVaiTroAttribute` thay thế `[Authorize]` của ASP.NET Identity.
- **Session giỏ hàng** — `SessionGioHang` (scoped service) serialize danh sách `GioHangItem` vào session JSON.
- **Auto migrate + seed** — `Program.cs` gọi `db.Database.Migrate()` và `DbSeeder.Seed(db)` khi khởi động.
- **Repository pattern** chưa áp dụng — controller inject trực tiếp `ApplicationDbContext`.
- View dùng Razor `@model` + Tag Helpers, validate server qua DataAnnotations, validate client qua `jquery-validation`.

## Roadmap

- [x] Khởi tạo dự án ASP.NET Core MVC
- [x] Kết nối PostgreSQL qua EF Core
- [x] Module Quản lý chủ đề (CRUD)
- [x] Schema đầy đủ (FullSchema migration)
- [x] Xác thực & phân quyền dựa trên Session
- [x] Module Sách, Tác giả, Nhà xuất bản, Nhóm sách (Admin)
- [x] Quản lý khách hàng & đơn hàng (Admin)
- [x] Dashboard & Thống kê (Admin)
- [x] Trang chủ & chi tiết sách (Customer)
- [x] Giỏ hàng (Session)
- [x] Đặt hàng & theo dõi đơn hàng (Customer)
- [x] Quản lý địa chỉ & số điện thoại (Customer)
- [x] Upload ảnh bìa sách (`AnhBiaHelper`)
- [x] Seed dữ liệu mẫu (`DbSeeder`)
- [ ] Hoàn thiện giao diện trang chủ (lọc theo chủ đề, tìm kiếm)
- [ ] Phân trang danh sách sách
- [ ] Gửi email đặt lại mật khẩu (hiện `ResetToken` chưa có email sender)
- [ ] Tích hợp thanh toán online (VNPay / Momo)
- [ ] Đánh giá & bình luận sách
- [ ] Tối ưu phân quyền Admin (hiện filter dựa trên session string)

## Ghi chú kỹ thuật

- `KiemTraVaiTroAttribute` kiểm tra vai trò từ session — cần đặt lên tất cả controller Admin trước khi triển khai.
- Bootstrap Icons (`bi bi-*`) được dùng trong view nhưng cần xác nhận đã thêm CDN vào `_Layout.cshtml`.
- `QuenMatKhauViewModel` có đủ field cho flow reset mật khẩu nhưng phần gửi email thực tế chưa được triển khai.
- Ảnh bìa lưu tại `wwwroot/uploads/bia/`; `AnhBiaHelper` xử lý lưu file và xóa file cũ khi cập nhật.

## Phụ thuộc

- [ASP.NET Core](https://learn.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [Npgsql EF Core Provider](https://www.npgsql.org/efcore/)
- [Bootstrap](https://getbootstrap.com/)
