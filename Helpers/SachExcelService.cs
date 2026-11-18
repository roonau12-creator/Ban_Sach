using System.Globalization;
using System.Xml;
using BanSach.Models;
using BanSach.Models.ViewModels;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;

namespace BanSach.Helpers
{
    public class SachExcelService
    {
        public const int SoDongNhapToiDa = 1000;

        private static readonly string[] Headers =
        {
            "Tên sách",
            "Mã chủ đề",
            "Mã nhóm",
            "Mã NXB",
            "Giá bán",
            "Số lượng tồn",
            "Ảnh bìa",
            "Mô tả",
            "Năm xuất bản",
            "Sách mới",
            "Nổi bật",
            "Mã tác giả (phân cách bằng dấu ;)"
        };

        public byte[] TaoMauNhap()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sach");
            GhiTieuDe(worksheet, Headers);
            worksheet.SheetView.FreezeRows(1);
            worksheet.Columns().AdjustToContents();

            var huongDan = workbook.Worksheets.Add("Hướng dẫn");
            huongDan.Cell(1, 1).Value = "Mỗi dòng trong sheet Sach là một sách mới.";
            huongDan.Cell(2, 1).Value = "Các mã chủ đề, nhóm, nhà xuất bản và tác giả phải tồn tại trong hệ thống.";
            huongDan.Cell(3, 1).Value = "Tên sách không được trùng với sách đã có hoặc dòng khác trong file.";
            huongDan.Cell(4, 1).Value = "Sách mới và Nổi bật nhận TRUE/FALSE; mã tác giả phân cách bằng dấu chấm phẩy.";
            huongDan.Column(1).AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] XuatExcel(IEnumerable<Sach> sachs)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách sách");
            var exportHeaders = new[]
            {
                "Mã sách",
                "Tên sách",
                "Mã chủ đề",
                "Chủ đề",
                "Mã nhóm",
                "Nhóm sách",
                "Mã NXB",
                "Nhà xuất bản",
                "Giá bán",
                "Số lượng tồn",
                "Ảnh bìa",
                "Mô tả",
                "Năm xuất bản",
                "Sách mới",
                "Nổi bật",
                "Mã tác giả (phân cách bằng dấu ;)",
                "Tác giả"
            };
            GhiTieuDe(worksheet, exportHeaders);

            var row = 2;
            foreach (var sach in sachs)
            {
                worksheet.Cell(row, 1).Value = sach.MaSach;
                worksheet.Cell(row, 2).Value = sach.TenSach;
                worksheet.Cell(row, 3).Value = sach.MaCD;
                worksheet.Cell(row, 4).Value = sach.ChuDe?.TenChuDe ?? string.Empty;
                worksheet.Cell(row, 5).Value = sach.MaNhom;
                worksheet.Cell(row, 6).Value = sach.NhomSach?.TenNhom ?? string.Empty;
                worksheet.Cell(row, 7).Value = sach.MaNXB;
                worksheet.Cell(row, 8).Value = sach.NhaXuatBan?.TenNXB ?? string.Empty;
                worksheet.Cell(row, 9).Value = sach.GiaBan;
                worksheet.Cell(row, 10).Value = sach.SoLuongTon;
                worksheet.Cell(row, 11).Value = sach.AnhBia ?? string.Empty;
                worksheet.Cell(row, 12).Value = sach.MoTa ?? string.Empty;
                worksheet.Cell(row, 13).Value = sach.NamXB?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
                worksheet.Cell(row, 14).Value = sach.SachMoi;
                worksheet.Cell(row, 15).Value = sach.NoiBat;
                worksheet.Cell(row, 16).Value = string.Join("; ", sach.SachTacGias.Select(x => x.MaTG));
                worksheet.Cell(row, 17).Value = string.Join("; ", sach.SachTacGias
                    .Where(x => x.TacGia != null)
                    .Select(x => x.TacGia!.TenTacGia));
                row++;
            }

            worksheet.SheetView.FreezeRows(1);
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public SachExcelImportPreviewViewModel DocNhap(Stream stream)
        {
            try
            {
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    return LoiTep("File Excel không có worksheet.");
                }

                var columns = DocCot(worksheet, out var errors);
                foreach (var header in Headers)
                {
                    if (!columns.ContainsKey(header))
                    {
                        errors.Add($"Thiếu cột bắt buộc: {header}.");
                    }
                }

                if (errors.Count > 0)
                {
                    return new SachExcelImportPreviewViewModel { FileErrors = errors };
                }

                var preview = new SachExcelImportPreviewViewModel();
                var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
                for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
                {
                    var values = Headers.ToDictionary(
                        header => header,
                        header => DocO(worksheet.Cell(rowNumber, columns[header])),
                        StringComparer.OrdinalIgnoreCase);
                    if (values.Values.All(string.IsNullOrWhiteSpace))
                    {
                        continue;
                    }

                    if (preview.Rows.Count >= SoDongNhapToiDa)
                    {
                        preview.FileErrors.Add($"File vượt quá giới hạn {SoDongNhapToiDa} dòng dữ liệu.");
                        break;
                    }

                    preview.Rows.Add(ParseDong(rowNumber, values));
                }

                if (preview.Rows.Count == 0 && preview.FileErrors.Count == 0)
                {
                    preview.FileErrors.Add("File không có dòng dữ liệu để nhập.");
                }

                return preview;
            }
            catch (Exception ex) when (
                ex is ArgumentException or
                IOException or
                InvalidDataException or
                NotSupportedException or
                XmlException or
                OpenXmlPackageException)
            {
                return LoiTep("Không thể đọc file Excel. Hãy tải mẫu và kiểm tra lại định dạng .xlsx.");
            }
        }

        private static Dictionary<string, int> DocCot(IXLWorksheet worksheet, out List<string> errors)
        {
            errors = new List<string>();
            var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var cell in worksheet.Row(1).CellsUsed())
            {
                var header = cell.GetString().Trim();
                if (header.Length == 0)
                {
                    continue;
                }

                if (!columns.TryAdd(header, cell.Address.ColumnNumber))
                {
                    errors.Add($"Cột \"{header}\" xuất hiện nhiều lần.");
                }
            }

            return columns;
        }

        private static SachExcelImportRowViewModel ParseDong(int rowNumber, IReadOnlyDictionary<string, string> values)
        {
            var row = new SachExcelImportRowViewModel
            {
                RowNumber = rowNumber,
                TenSach = values[Headers[0]].Trim(),
                AnhBia = values[Headers[6]],
                MoTa = values[Headers[7]]
            };

            if (string.IsNullOrWhiteSpace(row.TenSach))
            {
                row.Errors.Add("Tên sách là bắt buộc.");
            }
            else if (row.TenSach.Length > 250)
            {
                row.Errors.Add("Tên sách không được vượt quá 250 ký tự.");
            }

            row.MaCD = DocSoNguyen(rowNumber, Headers[1], values[Headers[1]], row.Errors, batBuoc: true);
            row.MaNhom = DocSoNguyen(rowNumber, Headers[2], values[Headers[2]], row.Errors, batBuoc: true);
            row.MaNXB = DocSoNguyen(rowNumber, Headers[3], values[Headers[3]], row.Errors, batBuoc: true);
            row.GiaBan = DocGia(rowNumber, values[Headers[4]], row.Errors);
            row.SoLuongTon = DocSoNguyen(rowNumber, Headers[5], values[Headers[5]], row.Errors, batBuoc: true);
            row.NamXB = DocSoNguyen(rowNumber, Headers[8], values[Headers[8]], row.Errors, batBuoc: false);
            row.SachMoi = DocBoolean(rowNumber, Headers[9], values[Headers[9]], row.Errors);
            row.NoiBat = DocBoolean(rowNumber, Headers[10], values[Headers[10]], row.Errors);
            row.MaTacGias = DocMaTacGias(rowNumber, values[Headers[11]], row.Errors);

            if (row.GiaBan.HasValue && row.GiaBan.Value < 0)
            {
                row.Errors.Add($"Dòng {rowNumber}: Giá bán phải lớn hơn hoặc bằng 0.");
            }
            else if (row.GiaBan.HasValue && row.GiaBan.Value > 9999999999999999.99m)
            {
                row.Errors.Add($"Dòng {rowNumber}: Giá bán vượt giới hạn numeric(18,2).");
            }

            if (row.SoLuongTon.HasValue && row.SoLuongTon.Value < 0)
            {
                row.Errors.Add($"Dòng {rowNumber}: Số lượng tồn phải lớn hơn hoặc bằng 0.");
            }

            if (row.AnhBia != null && row.AnhBia.Length > 300)
            {
                row.Errors.Add($"Dòng {rowNumber}: Đường dẫn ảnh bìa không được vượt quá 300 ký tự.");
            }

            return row;
        }

        private static int? DocSoNguyen(
            int rowNumber,
            string tenCot,
            string giaTri,
            List<string> errors,
            bool batBuoc)
        {
            if (string.IsNullOrWhiteSpace(giaTri))
            {
                if (batBuoc)
                {
                    errors.Add($"Dòng {rowNumber}: {tenCot} là bắt buộc.");
                }

                return null;
            }

            if (int.TryParse(giaTri, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            errors.Add($"Dòng {rowNumber}: {tenCot} phải là số nguyên (giá trị \"{giaTri}\").");
            return null;
        }

        private static decimal? DocGia(int rowNumber, string giaTri, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(giaTri))
            {
                errors.Add($"Dòng {rowNumber}: Giá bán là bắt buộc.");
                return null;
            }

            if (decimal.TryParse(giaTri, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
                || decimal.TryParse(giaTri, NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out value))
            {
                return value;
            }

            errors.Add($"Dòng {rowNumber}: Giá bán không hợp lệ (giá trị \"{giaTri}\").");
            return null;
        }

        private static bool DocBoolean(int rowNumber, string tenCot, string giaTri, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(giaTri))
            {
                return false;
            }

            if (bool.TryParse(giaTri, out var value))
            {
                return value;
            }

            if (giaTri == "1") return true;
            if (giaTri == "0") return false;

            errors.Add($"Dòng {rowNumber}: {tenCot} chỉ nhận TRUE hoặc FALSE.");
            return false;
        }

        private static List<int> DocMaTacGias(int rowNumber, string giaTri, List<string> errors)
        {
            var result = new List<int>();
            if (string.IsNullOrWhiteSpace(giaTri))
            {
                return result;
            }

            foreach (var item in giaTri.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (!int.TryParse(item, NumberStyles.Integer, CultureInfo.InvariantCulture, out var maTG))
                {
                    errors.Add($"Dòng {rowNumber}: Mã tác giả \"{item}\" không hợp lệ.");
                    continue;
                }

                if (result.Contains(maTG))
                {
                    errors.Add($"Dòng {rowNumber}: Mã tác giả {maTG} bị lặp.");
                    continue;
                }

                result.Add(maTG);
            }

            return result;
        }

        private static string DocO(IXLCell cell)
        {
            return cell.Value.Type switch
            {
                XLDataType.Blank => string.Empty,
                XLDataType.Number => cell.Value.GetNumber().ToString(CultureInfo.InvariantCulture),
                XLDataType.Boolean => cell.Value.GetBoolean() ? "TRUE" : "FALSE",
                _ => cell.GetString().Trim()
            };
        }

        private static void GhiTieuDe(IXLWorksheet worksheet, IReadOnlyList<string> headers)
        {
            for (var index = 0; index < headers.Count; index++)
            {
                worksheet.Cell(1, index + 1).Value = headers[index];
            }

            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            worksheet.Row(1).Style.Alignment.WrapText = true;
        }

        private static SachExcelImportPreviewViewModel LoiTep(string error)
        {
            return new SachExcelImportPreviewViewModel
            {
                FileErrors = new List<string> { error }
            };
        }
    }
}
