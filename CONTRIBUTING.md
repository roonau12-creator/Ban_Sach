# CONTRIBUTING.md

# QUY ĐỊNH ĐÓNG GÓP VÀ QUẢN LÝ PHÁT TRIỂN DỰ ÁN

**Tên dự án:** BanSach – Hệ thống quản lý và bán sách trực tuyến  
**Nền tảng:** ASP.NET Core MVC (.NET 10)  
**Ngôn ngữ lập trình:** C#  
**Cơ sở dữ liệu:** PostgreSQL  
**ORM:** Entity Framework Core  
**Quản lý mã nguồn:** Git và GitHub  

---

## 1. GIỚI THIỆU DỰ ÁN

BanSach là hệ thống quản lý và bán sách trực tuyến được phát triển nhằm hỗ trợ hoạt động kinh doanh sách, quản lý thông tin sản phẩm, khách hàng, đơn hàng và doanh thu.

Dự án được xây dựng theo mô hình ASP.NET Core MVC, sử dụng Entity Framework Core để làm việc với cơ sở dữ liệu PostgreSQL.

Repository được quản lý tập trung trên GitHub. Các thành viên tham gia dự án cần tuân thủ những quy định trong tài liệu này để đảm bảo quá trình phát triển thống nhất, minh bạch và có thể kiểm soát.

### 1.1. Mục đích của tài liệu

Tài liệu CONTRIBUTING được xây dựng nhằm:

- Xác định vai trò và trách nhiệm của từng thành viên.
- Quy định quy trình phân công và thực hiện công việc.
- Thống nhất cách sử dụng Git và GitHub.
- Kiểm soát chất lượng mã nguồn trước khi hợp nhất.
- Quản lý tiến độ thông qua GitHub Issues và Pull Requests.
- Hạn chế xung đột mã nguồn giữa các thành viên.
- Đảm bảo tính bảo mật, ổn định và khả năng bảo trì của hệ thống.

### 1.2. Phạm vi áp dụng

Quy định này áp dụng cho tất cả thành viên tham gia phát triển, kiểm thử, quản lý và bảo trì dự án BanSach.

Mọi thành viên cần đọc và tuân thủ tài liệu trước khi thực hiện thay đổi trên repository.

---

## 2. CÔNG NGHỆ VÀ MÔI TRƯỜNG PHÁT TRIỂN

### 2.1. Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Ngôn ngữ | C# |
| Framework | ASP.NET Core MVC .NET 10 |
| ORM | Entity Framework Core 10 |
| Database | PostgreSQL |
| Database Provider | Npgsql |
| Frontend | HTML, CSS, JavaScript |
| UI Framework | Bootstrap 5 |
| Source Control | Git |
| Repository | GitHub |

### 2.2. Yêu cầu môi trường

Thành viên cần chuẩn bị:

- .NET SDK 10.0.
- PostgreSQL.
- Visual Studio hoặc Visual Studio Code.
- Git.
- Tài khoản GitHub.
- Công cụ quản lý cơ sở dữ liệu như pgAdmin hoặc DBeaver.

### 2.3. Cài đặt và chạy dự án

Sau khi Clone repository, thực hiện:

```bash
git clone <repository-url>
cd BanSach
dotnet restore
dotnet ef database update
dotnet run
```

Thành viên cần kiểm tra ứng dụng hoạt động bình thường trước khi bắt đầu phát triển chức năng.

---

## 3. CƠ CẤU TỔ CHỨC VÀ PHÂN CÔNG TRÁCH NHIỆM

Để đảm bảo hiệu quả quản lý, dự án được tổ chức theo các vai trò sau:

### 3.1. Project Manager (PM) – Quản lý dự án

Project Manager chịu trách nhiệm điều phối và kiểm soát toàn bộ quá trình phát triển.

**Nhiệm vụ:**

- Xây dựng kế hoạch và xác định mục tiêu dự án.
- Phân chia công việc theo chức năng và năng lực thành viên.
- Tạo, quản lý và theo dõi GitHub Issues.
- Xác định mức độ ưu tiên cho từng nhiệm vụ.
- Theo dõi tiến độ và cập nhật trạng thái công việc.
- Kiểm tra các Pull Requests trước khi hợp nhất.
- Phối hợp xử lý xung đột và vấn đề phát sinh.
- Đánh giá kết quả thực hiện của các thành viên.
- Kiểm soát phạm vi và chất lượng sản phẩm.
- Đảm bảo tài liệu dự án được cập nhật đầy đủ.

**Quyền hạn:**

- Quản lý và phân công Issues.
- Xem xét, yêu cầu chỉnh sửa hoặc phê duyệt Pull Requests theo quyền được cấp.
- Đề xuất thay đổi kế hoạch phát triển.
- Quản lý các nhánh và quy trình làm việc theo chính sách repository.

### 3.2. Backend Developer – Lập trình viên Backend

**Nhiệm vụ:**

- Phát triển chức năng xử lý nghiệp vụ bằng C#.
- Xây dựng và quản lý Controller, Model và ViewModel.
- Xử lý kết nối cơ sở dữ liệu PostgreSQL.
- Làm việc với Entity Framework Core.
- Xây dựng các chức năng quản lý sách, khách hàng và đơn hàng.
- Kiểm tra tính chính xác của dữ liệu.
- Đảm bảo các chức năng hoạt động đúng yêu cầu.
- Viết code rõ ràng, dễ bảo trì.

### 3.3. Frontend Developer – Lập trình viên giao diện

**Nhiệm vụ:**

- Phát triển giao diện người dùng.
- Xây dựng các trang Razor View.
- Sử dụng HTML, CSS, JavaScript và Bootstrap.
- Đảm bảo giao diện tương thích với các kích thước màn hình.
- Tối ưu trải nghiệm người dùng.
- Phối hợp với Backend Developer để tích hợp dữ liệu.

### 3.4. Database Developer – Quản trị và phát triển cơ sở dữ liệu

**Nhiệm vụ:**

- Thiết kế cấu trúc cơ sở dữ liệu.
- Xác định khóa chính, khóa ngoại và quan hệ giữa các bảng.
- Quản lý các bảng và dữ liệu trong PostgreSQL.
- Tạo và quản lý Entity Framework Migrations.
- Kiểm tra tính toàn vẹn dữ liệu.
- Tối ưu các truy vấn khi cần thiết.
- Đảm bảo không làm mất dữ liệu hiện có khi cập nhật cấu trúc database.

### 3.5. Tester / QA – Kiểm thử chất lượng

**Nhiệm vụ:**

- Kiểm tra các chức năng sau khi lập trình.
- Xây dựng các trường hợp kiểm thử.
- Kiểm tra dữ liệu đầu vào và đầu ra.
- Phát hiện và ghi nhận lỗi trên GitHub Issues.
- Kiểm tra quyền truy cập Admin và Customer.
- Kiểm tra quy trình đăng nhập, đăng ký, giỏ hàng và đặt hàng.
- Kiểm thử lại sau khi Developer sửa lỗi.
- Xác nhận kết quả kiểm thử trước khi nghiệm thu.

**Lưu ý:** Một thành viên có thể đảm nhiệm nhiều vai trò tùy theo quy mô nhóm. Việc phân công phải được Project Manager thống nhất.

---

## 4. QUY TRÌNH QUẢN LÝ CÔNG VIỆC

Dự án sử dụng GitHub Issues và Pull Requests để quản lý công việc và theo dõi quá trình phát triển.

### 4.1. Tạo GitHub Issues

Mỗi chức năng mới, lỗi cần sửa hoặc yêu cầu cải tiến nên được ghi nhận bằng một Issue.

Một Issue cần có các thông tin:

- Tiêu đề công việc.
- Mô tả chi tiết.
- Mục tiêu thực hiện.
- Phạm vi công việc.
- Các bước thực hiện.
- Kết quả mong đợi.
- Người được giao nhiệm vụ.
- Mức độ ưu tiên.
- Labels phân loại.

**Ví dụ:**

Tiêu đề: `[FEATURE] Xây dựng chức năng quản lý sách`

Mô tả:

- Xây dựng giao diện danh sách sách.
- Thêm chức năng tạo sách mới.
- Chỉnh sửa thông tin sách.
- Xóa sách.
- Xem chi tiết sách.
- Kết nối dữ liệu với PostgreSQL.
- Kiểm tra dữ liệu đầu vào.

Kết quả mong đợi: Quản trị viên có thể quản lý thông tin sách thông qua khu vực Admin.

### 4.2. Phân loại Labels

Sử dụng Labels để phân loại và theo dõi công việc.

| Label | Ý nghĩa |
|---|---|
| `feature` | Phát triển chức năng mới |
| `bug` | Sửa lỗi hệ thống |
| `enhancement` | Cải tiến chức năng |
| `documentation` | Cập nhật tài liệu |
| `database` | Công việc liên quan cơ sở dữ liệu |
| `frontend` | Phát triển giao diện |
| `backend` | Xử lý nghiệp vụ |
| `testing` | Kiểm thử |
| `priority-high` | Ưu tiên cao |
| `priority-medium` | Ưu tiên trung bình |
| `priority-low` | Ưu tiên thấp |

### 4.3. Quy trình thực hiện công việc

Quy trình thống nhất:

1. Project Manager xác định công việc cần thực hiện.
2. Tạo GitHub Issue mô tả yêu cầu.
3. Giao Issue cho thành viên phụ trách.
4. Thành viên xác nhận nhiệm vụ.
5. Developer tạo nhánh làm việc.
6. Tiến hành phát triển và kiểm thử.
7. Commit và Push mã nguồn lên GitHub.
8. Tạo Pull Request.
9. Người phụ trách kiểm tra mã nguồn.
10. Phê duyệt và Merge khi đạt yêu cầu.
11. Đóng Issue sau khi công việc hoàn thành và được xác nhận.

---

## 5. QUY ĐỊNH QUẢN LÝ GIT BRANCH

Để tránh ảnh hưởng đến mã nguồn chính, các thành viên không được tự ý phát triển trực tiếp trên nhánh `main`.

### 5.1. Quy định các nhánh

| Nhánh | Chức năng |
|---|---|
| `main` | Mã nguồn chính thức, ổn định |
| `feature/*` | Phát triển chức năng mới |
| `bugfix/*` | Sửa lỗi thông thường |
| `hotfix/*` | Sửa lỗi khẩn cấp |
| `docs/*` | Cập nhật tài liệu |
| `refactor/*` | Tái cấu trúc mã nguồn |

### 5.2. Tạo nhánh mới

Trước khi bắt đầu công việc:

```bash
git checkout main
git pull origin main
git checkout -b feature/book-management
```

Ví dụ tên nhánh:

```text
feature/book-management
feature/customer-management
feature/order-management
feature/shopping-cart
bugfix/login-error
docs/update-readme
```

### 5.3. Quy định bảo vệ nhánh main

- Không Push trực tiếp mã nguồn chưa kiểm tra lên `main`.
- Mọi thay đổi quan trọng cần thông qua Pull Request.
- Cần kiểm tra lỗi trước khi Merge.
- Không tự ý ghi đè lịch sử commit của nhánh chính.
- Chỉ người có quyền phù hợp mới được Merge vào nhánh chính.

Nếu repository hỗ trợ Branch Protection Rules, Project Manager nên cấu hình yêu cầu Pull Request và kiểm tra trước khi Merge.

---

## 6. QUY ĐỊNH COMMIT VÀ PUSH MÃ NGUỒN

### 6.1. Nguyên tắc Commit

Mỗi Commit cần có nội dung rõ ràng, mô tả chính xác thay đổi.

Không sử dụng các nội dung chung chung như:

- update
- fix
- test
- sửa code
- hoàn thành

### 6.2. Quy chuẩn Commit Message

| Tiền tố | Ý nghĩa |
|---|---|
| `feat:` | Thêm chức năng |
| `fix:` | Sửa lỗi |
| `docs:` | Cập nhật tài liệu |
| `style:` | Điều chỉnh định dạng |
| `refactor:` | Tái cấu trúc code |
| `test:` | Thêm hoặc sửa kiểm thử |
| `chore:` | Công việc bảo trì |

**Ví dụ:**

```bash
git commit -m "feat: add book management"
git commit -m "fix: correct login validation"
git commit -m "docs: update project documentation"
git commit -m "test: add order processing tests"
```

### 6.3. Quy trình Push

```bash
git status
git add .
git commit -m "feat: add book management"
git push origin feature/book-management
```

Mỗi thành viên cần kiểm tra lại nhánh hiện tại trước khi Push để tránh đưa nhầm mã nguồn lên nhánh chính.

---

## 7. QUY TRÌNH PULL REQUEST VÀ CODE REVIEW

Pull Request (PR) là quy trình đề xuất hợp nhất mã nguồn từ nhánh phát triển vào nhánh chính.

### 7.1. Tạo Pull Request

Sau khi hoàn thành chức năng:

- Truy cập repository trên GitHub.
- Chọn Pull Requests.
- Chọn New Pull Request.
- Chọn nhánh nguồn là nhánh feature.
- Chọn nhánh đích là `main`.
- Viết tiêu đề và mô tả thay đổi.
- Liên kết Issue liên quan.
- Gửi Pull Request để xem xét.

### 7.2. Nội dung Pull Request

Mỗi PR cần cung cấp:

- Chức năng hoặc lỗi đã xử lý.
- Nội dung thay đổi.
- Các file đã chỉnh sửa.
- Kết quả kiểm thử.
- Hình ảnh giao diện nếu có thay đổi UI.
- Issue liên quan.

### 7.3. Code Review

Người kiểm tra cần đánh giá:

- Code có đúng yêu cầu không?
- Có lỗi cú pháp hoặc lỗi logic không?
- Có ảnh hưởng đến chức năng khác không?
- Có tuân thủ cấu trúc dự án không?
- Có đảm bảo an toàn dữ liệu không?
- Có chứa thông tin nhạy cảm không?
- Có thể chạy và kiểm thử được không?

### 7.4. Điều kiện Merge

Chỉ Merge khi:

- PR đã được kiểm tra.
- Các lỗi quan trọng đã được xử lý.
- Những thay đổi đáp ứng yêu cầu Issue.
- Không còn xung đột cần giải quyết.
- Đã được người có quyền phê duyệt theo quy định nhóm.

---

## 8. TIÊU CHUẨN PHÁT TRIỂN MÃ NGUỒN

### 8.1. Quy định chung

- Đặt tên biến, lớp và phương thức có ý nghĩa.
- Viết code rõ ràng, dễ đọc và dễ bảo trì.
- Không sử dụng mã nguồn không rõ nguồn gốc.
- Hạn chế code trùng lặp.
- Kiểm tra dữ liệu trước khi lưu vào database.
- Xử lý lỗi phù hợp.
- Không tự ý thay đổi cấu trúc chung khi chưa trao đổi với nhóm.

### 8.2. Quy định theo cấu trúc dự án

Các thành viên cần tuân thủ cấu trúc hiện có:

- `Areas/Admin`: Chức năng quản trị.
- `Areas/Customer`: Chức năng khách hàng.
- `Controllers`: Xử lý yêu cầu.
- `Models`: Các mô hình dữ liệu.
- `Models/ViewModels`: Dữ liệu phục vụ giao diện.
- `Data`: Quản lý DbContext.
- `Filters`: Kiểm tra đăng nhập và phân quyền.
- `Helpers`: Các thành phần hỗ trợ.
- `Migrations`: Lịch sử thay đổi cấu trúc database.
- `Views`: Giao diện Razor.
- `wwwroot`: CSS, JavaScript và tài nguyên tĩnh.

### 8.3. Quy định bảo mật

- Không lưu mật khẩu dạng văn bản thuần.
- Không công khai mật khẩu PostgreSQL.
- Không đưa API Key, Token hoặc thông tin bí mật lên GitHub.
- Kiểm tra quyền truy cập Admin và Customer.
- Không đưa dữ liệu cá nhân thực của khách hàng vào repository.
- Không chia sẻ thông tin đăng nhập tài khoản dự án.
- Sử dụng User Secrets hoặc biến môi trường cho thông tin cấu hình nhạy cảm.

---

## 9. QUẢN LÝ CƠ SỞ DỮ LIỆU

Dự án sử dụng PostgreSQL kết hợp Entity Framework Core.

Các thay đổi về cấu trúc dữ liệu cần được thực hiện có kiểm soát.

### 9.1. Quy định thay đổi database

- Trao đổi với người phụ trách cơ sở dữ liệu trước khi thay đổi cấu trúc quan trọng.
- Không tự ý xóa bảng hoặc cột đang được sử dụng.
- Kiểm tra quan hệ khóa chính và khóa ngoại.
- Đảm bảo dữ liệu có tính toàn vẹn.
- Cập nhật Migration khi thay đổi Model.
- Kiểm tra Migration trước khi đưa vào nhánh chính.

### 9.2. Lệnh Migration

Tạo Migration:

```bash
dotnet ef migrations add TenMigration
```

Cập nhật database:

```bash
dotnet ef database update
```

Kiểm tra quá trình Build:

```bash
dotnet build
```

Không sử dụng Migration có khả năng làm mất dữ liệu khi chưa được xem xét và chấp thuận.

---

## 10. QUY TRÌNH KIỂM THỬ VÀ NGHIỆM THU

Mọi chức năng mới hoặc chức năng được chỉnh sửa cần được kiểm tra trước khi hợp nhất.

### 10.1. Kiểm thử chức năng

Các chức năng cần được kiểm tra theo yêu cầu:

- Đăng ký và đăng nhập.
- Phân quyền Admin và Customer.
- Quản lý sách.
- Quản lý chủ đề và tác giả.
- Quản lý nhà xuất bản.
- Quản lý khách hàng.
- Giỏ hàng.
- Đặt hàng.
- Quản lý đơn hàng.
- Quản lý địa chỉ và số điện thoại.
- Thống kê doanh thu.

### 10.2. Tiêu chí nghiệm thu

Một chức năng được xem là hoàn thành khi:

- Hoạt động đúng yêu cầu.
- Không xuất hiện lỗi nghiêm trọng.
- Dữ liệu được lưu và truy xuất chính xác.
- Không ảnh hưởng đến chức năng hiện có.
- Được kiểm tra trên môi trường phát triển.
- Có tài liệu hoặc mô tả thay đổi cần thiết.
- Được người phụ trách xác nhận.

### 10.3. Báo cáo lỗi

Khi phát hiện lỗi, thành viên cần tạo GitHub Issue với:

- Tiêu đề lỗi.
- Mô tả tình trạng.
- Các bước tái hiện lỗi.
- Kết quả thực tế.
- Kết quả mong đợi.
- Hình ảnh hoặc thông báo lỗi nếu có.
- Mức độ ảnh hưởng.

---

## 11. QUẢN LÝ TIẾN ĐỘ VÀ BÁO CÁO

Project Manager có trách nhiệm theo dõi tiến độ thông qua GitHub Issues và Projects.

### 11.1. Trạng thái công việc

Các trạng thái đề xuất:

- `Todo`: Công việc chưa bắt đầu.
- `In Progress`: Đang thực hiện.
- `In Review`: Đang chờ kiểm tra.
- `Done`: Đã hoàn thành.
- `Blocked`: Đang gặp trở ngại.

### 11.2. Trách nhiệm báo cáo

Mỗi thành viên cần:

- Cập nhật trạng thái Issue.
- Thông báo khi hoàn thành nhiệm vụ.
- Báo cáo khó khăn hoặc lỗi phát sinh.
- Không tự ý đóng Issue khi chưa hoàn thành yêu cầu.
- Thông báo khi công việc có nguy cơ chậm tiến độ.

Project Manager cần theo dõi các công việc tồn đọng, đánh giá tiến độ và điều chỉnh phân công khi cần thiết.

---

## 12. QUY TẮC ỨNG XỬ VÀ PHỐI HỢP NHÓM

- Tôn trọng ý kiến và đóng góp của các thành viên.
- Không tự ý chỉnh sửa công việc của thành viên khác khi chưa trao đổi.
- Không sử dụng lời lẽ thiếu tôn trọng trong Issues và Pull Requests.
- Trao đổi rõ ràng khi xảy ra xung đột mã nguồn.
- Hỗ trợ thành viên khi gặp khó khăn kỹ thuật.
- Không cố tình đưa mã nguồn lỗi vào nhánh chính.
- Chịu trách nhiệm với nhiệm vụ đã nhận.

Mọi quyết định ảnh hưởng đến kiến trúc, cơ sở dữ liệu hoặc phạm vi dự án cần được trao đổi với người phụ trách trước khi thực hiện.

---

## 13. QUY TRÌNH PHÁT HÀNH VÀ CẬP NHẬT

Trước khi phát hành phiên bản mới:

- Kiểm tra các chức năng đã hoàn thành.
- Kiểm tra lỗi còn tồn đọng.
- Xác nhận các thay đổi đã được Merge.
- Cập nhật README nếu có thay đổi cách cài đặt hoặc sử dụng.
- Ghi nhận các thay đổi trong CHANGELOG nếu dự án sử dụng.
- Tạo GitHub Release khi cần thiết.

Không phát hành phiên bản khi còn lỗi nghiêm trọng ảnh hưởng đến hoạt động của hệ thống.

---

## 14. QUY ĐỊNH VỀ QUYỀN SỞ HỮU VÀ GIẤY PHÉP

- Các thành viên cần tôn trọng quyền sở hữu trí tuệ đối với mã nguồn.
- Không sao chép mã nguồn trái phép.
- Khi sử dụng thư viện bên ngoài cần tuân thủ giấy phép tương ứng.
- Các đóng góp cần đảm bảo không vi phạm quyền của bên thứ ba.
- Việc sử dụng và phân phối mã nguồn phải tuân thủ file LICENSE của repository.

---

## 15. CHECKLIST DÀNH CHO THÀNH VIÊN

Trước khi gửi Pull Request, thành viên cần kiểm tra:

- [ ] Đã đọc tài liệu CONTRIBUTING.md.
- [ ] Đã nhận và hiểu rõ GitHub Issue.
- [ ] Đã tạo đúng nhánh làm việc.
- [ ] Đã cập nhật mã nguồn mới nhất.
- [ ] Đã hoàn thành chức năng được giao.
- [ ] Đã kiểm tra lỗi cơ bản.
- [ ] Đã chạy thử ứng dụng.
- [ ] Đã kiểm tra dữ liệu PostgreSQL nếu có thay đổi.
- [ ] Không đưa thông tin nhạy cảm lên GitHub.
- [ ] Commit Message rõ ràng.
- [ ] Đã Push đúng nhánh.
- [ ] Đã tạo Pull Request và liên kết Issue.

---

## 16. KẾT LUẬN

Tài liệu CONTRIBUTING là cơ sở thống nhất quy trình làm việc và đóng góp mã nguồn cho dự án BanSach.

Tất cả thành viên tham gia dự án có trách nhiệm tuân thủ các quy định nhằm đảm bảo quá trình phát triển được tổ chức khoa học, minh bạch và hiệu quả.

Mọi thay đổi về quy trình đóng góp cần được Project Manager và các thành viên liên quan trao đổi, thống nhất trước khi áp dụng.

**BanSach Project – Development Team**

