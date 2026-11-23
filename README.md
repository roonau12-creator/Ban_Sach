# BanSach

## Khởi tạo phân quyền

1. Cấu hình chuỗi kết nối PostgreSQL trong `ConnectionStrings:DefaultConnection`.
2. Áp dụng migration: `dotnet ef database update`.
3. Trước lần chạy đầu tiên, cấu hình tài khoản quản trị ban đầu bằng các biến môi trường `BootstrapAdmin__Email` và `BootstrapAdmin__Password`. Mật khẩu bootstrap cần từ 12 đến 128 ký tự. Không lưu mật khẩu này vào mã nguồn hoặc file cấu hình được commit. Tài khoản đã tồn tại với email này sẽ được cấp lại vai trò quản trị khi ứng dụng khởi động.
4. Chạy ứng dụng. Người dùng đăng ký mới luôn nhận vai trò Khách hàng; chỉ quản trị viên mới có thể đổi vai trò trong mục Quản lý tài khoản.

## Vai trò và policy

- `Administrator`: quản lý nhân viên, cấu hình hệ thống, xem báo cáo doanh thu và quản lý sách.
- `Employee`: quản lý sách.
- `Customer`: nghiệp vụ khách hàng, không có quyền truy cập khu vực quản trị.

Các controller quản lý sách phải dùng policy `ManageBooks`; quản lý nhân viên dùng `ManageStaff`; cấu hình dùng `ManageSystemSettings`; xem doanh thu dùng `ViewRevenueReports`. Việc ẩn liên kết trên giao diện chỉ nhằm cải thiện trải nghiệm; quyền thực tế được kiểm tra trên máy chủ.

## Danh mục và sách

- Nhân viên và quản trị viên có thể thêm, sửa, xem và quản lý danh mục tại **Quản lý danh mục**; tên danh mục được chuẩn hóa khoảng trắng và kiểm tra trùng không phân biệt hoa/thường.
- Sách được gán vào một danh mục bắt buộc tại **Quản lý sách**. Danh mục đang có sách tham chiếu không thể bị xóa; hãy chuyển sách sang danh mục khác trước để giữ nguyên liên kết.
- Trang **Mua sách** hiển thị các danh mục, mô tả và sách theo đúng danh mục.
- Áp dụng migration mới bằng `dotnet ef database update` trước khi sử dụng các màn hình này.
