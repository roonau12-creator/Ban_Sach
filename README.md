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
