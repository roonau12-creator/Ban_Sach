using BanSach.Helpers;
using BanSach.Models;
using ClosedXML.Excel;

namespace BanSach.Tests.Helpers
{
    public class SachExcelServiceTests
    {
        private readonly SachExcelService _service = new();

        [Fact]
        public void DocNhap_FileHopLe_TraVeDuLieuDaDoc()
        {
            using var templateStream = new MemoryStream(_service.TaoMauNhap());
            using var workbook = new XLWorkbook(templateStream);
            var worksheet = workbook.Worksheet("Sach");
            worksheet.Cell(2, 1).Value = "Sách thử";
            worksheet.Cell(2, 2).Value = 1;
            worksheet.Cell(2, 3).Value = 2;
            worksheet.Cell(2, 4).Value = 3;
            worksheet.Cell(2, 5).Value = 125000m;
            worksheet.Cell(2, 6).Value = 10;
            worksheet.Cell(2, 10).Value = true;
            worksheet.Cell(2, 12).Value = "4; 5";

            using var input = new MemoryStream();
            workbook.SaveAs(input);
            input.Position = 0;

            var preview = _service.DocNhap(input);
            var row = Assert.Single(preview.Rows);

            Assert.True(preview.CanConfirm);
            Assert.Equal("Sách thử", row.TenSach);
            Assert.Equal(125000m, row.GiaBan);
            Assert.Equal(new[] { 4, 5 }, row.MaTacGias);
            Assert.True(row.SachMoi);
        }

        [Fact]
        public void DocNhap_GiaBanKhongHopLe_TraVeLoiTheoDong()
        {
            using var templateStream = new MemoryStream(_service.TaoMauNhap());
            using var workbook = new XLWorkbook(templateStream);
            var worksheet = workbook.Worksheet("Sach");
            worksheet.Cell(2, 1).Value = "Sách thử";
            worksheet.Cell(2, 2).Value = 1;
            worksheet.Cell(2, 3).Value = 2;
            worksheet.Cell(2, 4).Value = 3;
            worksheet.Cell(2, 5).Value = "không phải số";
            worksheet.Cell(2, 6).Value = 10;

            using var input = new MemoryStream();
            workbook.SaveAs(input);
            input.Position = 0;

            var preview = _service.DocNhap(input);

            Assert.False(preview.CanConfirm);
            Assert.Contains(preview.Rows.Single().Errors, error => error.Contains("Giá bán không hợp lệ"));
        }

        [Fact]
        public void DocNhap_ThieuCotBatBuoc_TraVeLoiCauTruc()
        {
            using var workbook = new XLWorkbook();
            workbook.Worksheets.Add("Sach").Cell(1, 1).Value = "Tên sách";
            using var input = new MemoryStream();
            workbook.SaveAs(input);
            input.Position = 0;

            var preview = _service.DocNhap(input);

            Assert.Contains(preview.FileErrors, error => error.Contains("Thiếu cột bắt buộc"));
            Assert.Empty(preview.Rows);
        }

        [Fact]
        public void XuatExcel_GhiDayDuTruongSachVaTacGia()
        {
            var sach = new Sach
            {
                MaSach = 10,
                TenSach = "Sách thử",
                MaCD = 1,
                ChuDe = new ChuDe { TenChuDe = "Chủ đề" },
                MaNhom = 2,
                NhomSach = new NhomSach { TenNhom = "Nhóm" },
                MaNXB = 3,
                NhaXuatBan = new NhaXuatBan { TenNXB = "Nhà xuất bản" },
                GiaBan = 125000m,
                SoLuongTon = 4,
                NamXB = 2024,
                SachMoi = true,
                SachTacGias = new List<SachTacGia>
                {
                    new() { MaTG = 7, TacGia = new TacGia { TenTacGia = "Tác giả" } }
                }
            };

            using var stream = new MemoryStream(_service.XuatExcel(new[] { sach }));
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet("Danh sách sách");

            Assert.Equal("Mã sách", worksheet.Cell(1, 1).GetString());
            Assert.Equal("Sách thử", worksheet.Cell(2, 2).GetString());
            Assert.Equal(125000d, worksheet.Cell(2, 9).Value.GetNumber());
            Assert.Equal("7", worksheet.Cell(2, 16).GetString());
            Assert.Equal("Tác giả", worksheet.Cell(2, 17).GetString());
        }
    }
}
