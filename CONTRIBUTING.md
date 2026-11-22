# Đóng góp cho BanSach

Cảm ơn bạn đã quan tâm đóng góp cho BanSach! Tài liệu này hướng dẫn cách thiết lập môi trường, quy ước code và quy trình gửi đóng góp.

## Quy tắc ứng xử

- Tôn trọng, lịch sự với mọi người trong issue, pull request và review.
- Góp ý hướng vào code, không hướng vào cá nhân.
- Không spam, không quảng cáo trong issue/PR.

## Yêu cầu môi trường

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- PostgreSQL đang chạy (mặc định `localhost:5432`, database `BanSach`)
- `dotnet-ef` tool (nếu chưa có):

```bash
dotnet tool install --global dotnet-ef
```

## Thiết lập môi trường phát triển

```bash
# 1. Clone repo
git clone <repo-url>
cd BanSach

# 2. Cài package
dotnet restore

# 3. Cấu hình chuỗi kết nối (không commit mật khẩu thật vào source)
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=BanSach;Username=postgres;Password=<mat-khau>"

# 4. Áp dụng migration (tự seed dữ liệu mẫu)
dotnet ef database update

# 5. Chạy ứng dụng
dotnet run
```

Ứng dụng chạy tại `http://localhost:5126` và `https://localhost:7217`. Khi khởi động, `Program.cs` tự động chạy `Database.Migrate()` và `DbSeeder.Seed(db)`.

## Quy trình đóng góp

1. **Tạo issue** (hoặc bình luận vào issue có sẵn) mô tả vấn đề/tính năng trước khi làm việc lớn.
2. **Fork** repo và tạo nhánh mới từ `dev` (nhánh phát triển):

   ```bash
   git checkout dev
   git pull origin dev
   git checkout -b feat/ten-tinh-nang
   ```

3. **Code** theo quy ước bên dưới, thêm test nếu cần.
4. **Kiểm tra cục bộ**: build + test phải pass.

   ```bash
   dotnet build
   dotnet test
   ```

5. **Commit** theo quy ước commit (bên dưới).
6. **Push** nhánh và mở **Pull Request** vào `dev` (không tạo PR trực tiếp vào `main`, trừ hotfix khẩn cấp).

### Quy ước đặt tên nhánh

| Tiền tố | Mục đích | Ví dụ |
| --- | --- | --- |
| `feat/` | Tính năng mới | `feat/phan-trang-danh-sach-sach` |
| `fix/` | Sửa bug | `fix/loi-xoa-anh-bia` |
| `refactor/` | Tái cấu trúc, không đổi hành vi | `refactor/session-gio-hang` |
| `docs/` | Tài liệu | `docs/cap-nhat-readme` |
| `test/` | Bổ sung test | `test/anh-bia-helper` |
| `chore/` | Việc lặt vặt (CI, config, dependabot...) | `chore/nang-cap-package` |

### Quy ước commit

Dùng dạng **Conventional Commits**, tiêu đề ngắn gọn, thì hiện tại, có tiền tố:

```
feat: thêm phân trang danh sách sách
fix: sửa lỗi cập nhật số lượng giỏ hàng âm
docs: bổ sung hướng dẫn migration
refactor: tách logic upload ảnh bìa khỏi controller
test: thêm test cho SessionGioHang
chore: cập nhật CI lên .NET 10
```

- Mỗi commit nên làm một việc rõ ràng.
- Không commit các file build (`bin/`, `obj/`), file cấu hình chứa mật khẩu, hay ảnh upload thử nghiệp.

## Quy ước code

### C# / ASP.NET Core

- Giữ nhất quán với code hiện có: **tên domain bằng tiếng Việt** (`Sach`, `ChuDe`, `DonHang`, `KiemTraVaiTroAttribute`, ...). Không dịch sang tiếng Anh khi thêm model/controller mới.
- PascalCase cho class/method/property, camelCase cho biến cục bộ/tham số, `_` cho field private nếu cần.
- Bật `Nullable` — xử lý cảnh báo null thay vì bỏ qua.
- Validate dữ liệu bằng **DataAnnotations** trên ViewModel; validate client tự động qua `jquery-validation`.
- Controller hiện inject trực tiếp `ApplicationDbContext` (chưa dùng repository pattern). Hãy theo đúng pattern này để đồng nhất, trừ khi có thảo luận thống nhất trong issue.
- Phân quyền dùng `KiemTraDangNhapAttribute` và `KiemTraVaiTroAttribute` (session-based, không dùng ASP.NET Identity). Nhớ gắn `KiemTraVaiTro` cho controller Admin mới.
- Hằng số vai trò/trạng thái/session keys đặt trong `Helpers/HangSo.cs`, không hardcode chuỗi rải rác.

### View (Razor)

- Dùng Tag Helpers và `@model` như các view hiện có.
- Giao diện Bootstrap 5.3.3; tái sử dụng class có sẵn trong `wwwroot/css/site.css` trước khi thêm CSS mới.
- View của Admin đặt trong `Areas/Admin/Views/<Controller>/`, của Customer trong `Areas/Customer/Views/<Controller>/`.

### Bảo mật

- **Không bao giờ** commit mật khẩu, connection string thật, hay token. Dùng `dotnet user-secrets` hoặc biến môi trường.
- Mật khẩu người dùng phải được hash trước khi lưu (xem `TaiKhoan.MatKhauHash`).
- Kiểm tra quyền truy cập ở server (filter), không chỉ ẩn nút trên giao diện.

## Migration (Entity Framework Core)

- Chỉ tạo migration khi có thay đổi schema:

  ```bash
  dotnet ef migrations add <TenMigrationRoNghia>   # ví dụ: ThemBangDanhGia
  ```

- **Không sửa migration đã được merge** vào `dev`/`main`. Nếu cần thay đổi, tạo migration mới.
- Đặt tên migration rõ nghĩa, không dùng tên chung chung như `Update1`.
- Pull request có migration phải ghi chú rõ trong mô tả (bảng/cột thay đổi, có cần seed lại không).

## Kiểm thử

- Test dùng **xUnit**, nằm trong `BanSach.Tests/` (mirror cấu trúc thư mục của project chính).

  ```bash
  dotnet test                                        # chạy toàn bộ
  dotnet test --filter "FullyQualifiedName~SessionGioHang"   # chạy một nhóm test
  ```

- Thêm test cho logic mới ở `Helpers/`, `Models/`, hoặc logic nghiệp vụ quan trọng (giỏ hàng, tính tiền, trạng thái đơn hàng...).
- Không cần test cho CRUD thuần túy hoặc view, trừ khi được yêu cầu trong issue.

## CI

Mọi push/PR vào `main` và `dev` đều chạy:

- **GitHub Actions** (`.github/workflows/ci.yml`): restore → build → test → publish.
- **Jenkins** (`Jenkinsfile`): pipeline tương đương restore → build → test → publish.

Yêu cầu bắt buộc trước khi PR được merge:

- [ ] `dotnet build` không có warning/error mới
- [ ] `dotnet test` pass toàn bộ
- [ ] CI (GitHub Actions/Jenkins) xanh

## Pull Request

- Tiêu đề PR ngắn gọn theo cùng quy ước commit (ví dụ `feat: thêm phân trang danh sách sách`).
- Mô tả PR gồm:
  - Vấn đề/tính năng và issue liên quan (nếu có, ghi `Closes #<so-issue>`).
  - Cách giải quyết, điểm cần lưu ý khi review.
  - Ảnh chụp màn hình trước/sau nếu thay đổi giao diện.
  - Ghi chú migration/seed nếu có thay đổi database.
- PR nên nhỏ, tập trung một mục đích; tránh trộn nhiều tính năng vào một PR.
- Giữ lịch sử commit sạch (squash/rebase nếu được yêu cầu), không force-push lên nhánh chung.
- Người review có thể yêu cầu thay đổi — hãy phản hồi trong vòng vài ngày để PR không bị đóng do không hoạt động.

## Báo cáo bug & đề xuất tính năng

Mở GitHub Issue với các thông tin:

- **Mô tả ngắn** vấn đề hoặc tính năng mong muốn.
- **Các bước tái hiện** (đối với bug): từng bước, URL, tài khoản vai trò (Admin/Customer).
- **Kết quả mong đợi** và **kết quả thực tế** (kèm ảnh chụp/log nếu có).
- **Môi trường**: OS, phiên bản .NET, PostgreSQL.

## Câu hỏi?

Nếu còn thắc mắc về quy trình hoặc kiến trúc, hãy mở issue với nhãn `question` hoặc bình luận trực tiếp trong pull request liên quan.
