# BanSach

> Hệ thống quản lý bán sách — ASP.NET Core MVC + Entity Framework Core + PostgreSQL

BanSach là web app quản lý và bán sách, viết bằng C# trên nền tảng ASP.NET Core MVC (kiến trúc Areas), sử dụng Entity Framework Core kết nối PostgreSQL và giao diện Bootstrap 5.

Trạng thái hiện tại: dự án đang phát triển, đã hoàn thành module **Quản lý chủ đề (ChuDe)** cho khu vực Admin.

## Công nghệ

| Thành phần | Phiên bản |
| --- | --- |
| .NET | 10.0 (`net10.0`) |
| ASP.NET Core MVC | .NET 10 (minimal hosting, `Program.cs`) |
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

# 2. Tạo database và áp dụng migration
dotnet ef database update

# 3. Chạy ứng dụng
dotnet run
```

Ứng dụng khởi động tại:

- http://localhost:5126
- https://localhost:7217

Nếu `dotnet ef` chưa có, cài tool toàn cục:

```bash
dotnet tool install --global dotnet-ef
```

## Cấu hình

Chuỗi kết nối nằm trong `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=BanSach;Username=postgres;Password=123"
}
```

> **Lưu ý:** mật khẩu đang để trực tiếp trong `appsettings.json`. Khi triển khai thực tế nên chuyển sang user-secrets hoặc biến môi trường:
>
> ```bash
> dotnet user-secrets init
> dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Password=..."
> ```

## Cấu trúc dự án

```
BanSach/
├── Areas/
│   ├── Admin/                 # Khu vực quản trị
│   │   ├── Controllers/
│   │   │   └── ChuDeController.cs
│   │   └── Views/ChuDe/       # Index, Create, Edit, Details, Delete
│   └── Customer/              # Khu vực khách hàng
│       ├── Controllers/HomeController.cs
│       └── Views/Home/        # Index, Privacy
├── Data/
│   └── ApplicationDbContext.cs
├── Helpers/                   # (nhánh dev) SessionGioHang, AnhBiaHelper, DbSeeder, HangSo
├── Migrations/                # Migration của EF Core
├── Models/                    # ChuDe, ErrorViewModel, ...
├── Views/Shared/              # _Layout, Error, _ValidationScriptsPartial
├── wwwroot/
│   ├── css/  js/              # site.css, site.js
│   ├── lib/                   # bootstrap, jquery, jquery-validation
│   └── uploads/bia/           # ảnh bìa sách
├── Program.cs                 # Khởi tạo service + định tuyến
└── appsettings.json
```

### Định tuyến

`Program.cs` khai báo 2 route:

| Route | Pattern | Mô tả |
| --- | --- | --- |
| `MyAreas` | `{area:exists}/{controller=Home}/{action=Index}/{id?}` | Truy cập trực tiếp area: `/Admin/ChuDe` |
| `default` | `{controller}/{action}/{id?}` (mặc định `area = Customer`) | `/`, `/Home/Index` → khu vực Customer |

## Chức năng hiện có

### Admin — Quản lý chủ đề (`/Admin/ChuDe`)

| Action | Đường dẫn | Mô tả |
| --- | --- | --- |
| `Index` | `GET /Admin/ChuDe` | Danh sách chủ đề, mới nhất trước |
| `Details` | `GET /Admin/ChuDe/Details/{id}` | Chi tiết một chủ đề |
| `Create` | `GET/POST /Admin/ChuDe/Create` | Thêm chủ đề |
| `Edit` | `GET/POST /Admin/ChuDe/Edit/{id}` | Cập nhật chủ đề |
| `Delete` | `GET/POST /Admin/ChuDe/Delete/{id}` | Xóa chủ đề (có xác nhận) |

Model `ChuDe` (`Models/ChuDe.cs`):

```csharp
public class ChuDe
{
    [Key] public int MaCD { get; set; }

    [Required]
    [StringLength(100)]
    public string TenChuDe { get; set; }
}
```

Các action ghi dữ liệu đều dùng `[HttpPost]` + `[ValidateAntiForgeryToken]` và báo kết quả qua `TempData["success"]`.

### Customer

- `GET /` — trang chủ
- `GET /Home/Privacy` — trang chính sách bảo mật (nội dung mặc định)
- `GET /Home/Error` — trang lỗi (khi chạy ở môi trường Production)

## Lệnh thường dùng

```bash
dotnet run                        # Chạy ứng dụng
dotnet build                      # Build
dotnet ef migrations add <Ten>   # Tạo migration
dotnet ef database update        # Áp dụng migration
dotnet ef database drop          # Xóa database
```

## Kiến trúc

- **Areas** tách khu vực quản trị và khách hàng. Cả hai dùng chung layout `Views/Shared/_Layout.cshtml`.
- **Repository pattern** chưa áp dụng — controller inject trực tiếp `ApplicationDbContext`.
- View dùng Razor `@model` kết hợp Tag Helpers, validate phía server qua DataAnnotations và phía client qua `jquery-validation`.

## Roadmap

- [x] Khởi tạo dự án ASP.NET Core MVC
- [x] Kết nối PostgreSQL qua EF Core
- [x] Module Quản lý chủ đề (CRUD)
- [ ] Xác thực & phân quyền (đăng nhập admin/khách, middleware `UseAuthentication`)
- [ ] Module Sách, Tác giả, Nhà xuất bản, Nhóm sách
- [ ] Giỏ hàng & đặt hàng (`SessionGioHang`)
- [ ] Đơn hàng, giao hàng, thống kê
- [ ] Upload ảnh bìa sách (`AnhBiaHelper`)
- [ ] Dashboard Admin

## Ghi chú kỹ thuật

Các vấn đề đang tồn tại, dự kiến xử lý ở các commit tiếp theo:

- Admin area chưa có `[Authorize]` — cần bổ sung xác thực trước khi triển khai.
- `Areas/Admin/Views/ChuDe/Create.cshtml` hiện trùng nội dung với `Index.cshtml`, cần viết lại form thêm mới.
- Bootstrap Icons được dùng trong view (`bi bi-*`) nhưng chưa thêm vào `wwwroot/lib` / `_Layout.cshtml`.
- `wwwroot/uploads/bia/` đã có sẵn ảnh nhưng chưa có model/upload endpoint sử dụng.

## Công nghệ phụ thuộc

- [ASP.NET Core](https://learn.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [Npgsql EF Core Provider](https://www.npgsql.org/efcore/)
- [Bootstrap](https://getbootstrap.com/)
