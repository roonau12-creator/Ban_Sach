using System.Globalization;
using System.Text;
using ClosedXML.Excel;

namespace BanSach.Helpers
{
    /// <summary>Một dòng sách đọc được từ file Excel (đã kiểm tra kiểu dữ liệu).</summary>
    public class DongSachExcel
    {
        public int Dong { get; set; }
        public string TenSach { get; set; } = string.Empty;
        public string ChuDe { get; set; } = string.Empty;
        public string NhomSach { get; set; } = string.Empty;
        public string NhaXuatBan { get; set; } = string.Empty;
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }
        public int? NamXB { get; set; }
        public List<string> TacGias { get; set; } = new();
        public string? MoTa { get; set; }
        public bool SachMoi { get; set; }
        public bool NoiBat { get; set; }
        public string? AnhBia { get; set; }
    }

    public class KetQuaDocExcel
    {
        public List<DongSachExcel> Dongs { get; } = new();
        public List<string> Loi { get; } = new();
    }

    public static class SachExcelHelper
    {
        public const string TenSach = "Tên sách";
        public const string ChuDe = "Chủ đề";
        public const string NhomSach = "Nhóm sách";
        public const string NhaXuatBan = "Nhà xuất bản";
        public const string GiaBan = "Giá bán";
        public const string SoLuongTon = "Số lượng tồn";
        public const string NamXB = "Năm xuất bản";
        public const string TacGia = "Tác giả";
        public const string MoTa = "Mô tả";
        public const string SachMoi = "Sách mới";
        public const string NoiBat = "Nổi bật";
        public const string AnhBia = "Ảnh bìa (URL)";

        public static readonly string[] CacCot =
        {
            TenSach, ChuDe, NhomSach, NhaXuatBan, GiaBan, SoLuongTon,
            NamXB, TacGia, MoTa, SachMoi, NoiBat, AnhBia
        };

        private static readonly string[] CotBatBuoc = { TenSach, ChuDe, NhomSach, NhaXuatBan, GiaBan };

        public static byte[] TaoFileMau(
            IEnumerable<string> chuDes, IEnumerable<string> nhomSachs,
            IEnumerable<string> nhaXuatBans, IEnumerable<string> tacGias)
        {
            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Sach");

            for (var i = 0; i < CacCot.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = CacCot[i] + (CotBatBuoc.Contains(CacCot[i]) ? " *" : "");
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#8b3a2a");
                cell.Style.Font.FontColor = XLColor.White;
            }

            object[] viDu =
            {
                "Đắc nhân tâm", "Kỹ năng sống", "Sách bán chạy", "NXB Tổng hợp TP.HCM",
                86000, 50, 2020, "Dale Carnegie", "Cuốn sách kinh điển về nghệ thuật giao tiếp.",
                "Có", "Không", ""
            };
            for (var i = 0; i < viDu.Length; i++)
            {
                ws.Cell(2, i + 1).Value = XLCellValue.FromObject(viDu[i]);
                ws.Cell(2, i + 1).Style.Font.Italic = true;
                ws.Cell(2, i + 1).Style.Font.FontColor = XLColor.Gray;
            }

            ws.SheetView.FreezeRows(1);
            ws.Columns().AdjustToContents(1, 2, 12, 45);

            var hd = wb.AddWorksheet("Huong dan");
            var dong = 1;
            string[] huongDan =
            {
                "HƯỚNG DẪN NHẬP SÁCH",
                "• Nhập mỗi cuốn sách trên một dòng của sheet \"Sach\", bắt đầu từ dòng 2 (xóa dòng ví dụ trước khi nhập).",
                "• Cột có dấu * là bắt buộc.",
                "• Chủ đề, Nhóm sách, Nhà xuất bản: ghi đúng tên đã có trong hệ thống (xem các cột bên dưới).",
                "• Tác giả: nhiều tác giả cách nhau bằng dấu phẩy hoặc dấu chấm phẩy.",
                "• Giá bán: số, không âm (vd: 86000). Số lượng tồn và Năm xuất bản: số nguyên.",
                "• Sách mới / Nổi bật: ghi \"Có\" hoặc \"Không\" (để trống = Không).",
                "• Ảnh bìa: đường dẫn ảnh (tùy chọn)."
            };
            foreach (var line in huongDan) hd.Cell(dong++, 1).Value = line;
            hd.Cell(1, 1).Style.Font.Bold = true;

            dong++;
            GhiDanhSach(hd, dong, 1, "Chủ đề hiện có", chuDes);
            GhiDanhSach(hd, dong, 2, "Nhóm sách hiện có", nhomSachs);
            GhiDanhSach(hd, dong, 3, "Nhà xuất bản hiện có", nhaXuatBans);
            GhiDanhSach(hd, dong, 4, "Tác giả hiện có", tacGias);
            hd.Column(1).Width = 40;
            hd.Columns(2, 4).Width = 30;

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        private static void GhiDanhSach(IXLWorksheet ws, int dong, int cot, string tieuDe, IEnumerable<string> items)
        {
            ws.Cell(dong, cot).Value = tieuDe;
            ws.Cell(dong, cot).Style.Font.Bold = true;
            foreach (var item in items) ws.Cell(++dong, cot).Value = item;
        }

        public static KetQuaDocExcel DocFile(Stream stream)
        {
            var kq = new KetQuaDocExcel();
            XLWorkbook wb;
            try
            {
                wb = new XLWorkbook(stream);
            }
            catch (Exception)
            {
                kq.Loi.Add("Không đọc được file. Vui lòng dùng file Excel định dạng .xlsx.");
                return kq;
            }

            using (wb)
            {
                var ws = wb.Worksheets.FirstOrDefault(w => w.Name == "Sach") ?? wb.Worksheet(1);
                var headerRow = ws.FirstRowUsed();
                if (headerRow == null)
                {
                    kq.Loi.Add("File không có dữ liệu.");
                    return kq;
                }

                // Xác định vị trí từng cột theo tiêu đề (không phân biệt hoa thường, dấu *)
                var viTri = new Dictionary<string, int>();
                foreach (var cell in headerRow.CellsUsed())
                {
                    var ten = ChuanHoa(cell.GetString().Replace("*", ""));
                    var cot = CacCot.FirstOrDefault(c => ChuanHoa(c) == ten);
                    if (cot != null && !viTri.ContainsKey(cot)) viTri[cot] = cell.Address.ColumnNumber;
                }

                var thieu = CotBatBuoc.Where(c => !viTri.ContainsKey(c)).ToList();
                if (thieu.Count > 0)
                {
                    kq.Loi.Add("Thiếu cột bắt buộc: " + string.Join(", ", thieu) + ". Hãy dùng file mẫu.");
                    return kq;
                }

                var lastRow = ws.LastRowUsed()!.RowNumber();
                for (var r = headerRow.RowNumber() + 1; r <= lastRow; r++)
                {
                    var row = ws.Row(r);
                    string Lay(string cot) => viTri.TryGetValue(cot, out var c) ? row.Cell(c).GetFormattedString().Trim() : "";

                    if (CacCot.All(c => string.IsNullOrEmpty(Lay(c)))) continue; // bỏ dòng trống

                    var loiDong = new List<string>();
                    var d = new DongSachExcel
                    {
                        Dong = r,
                        TenSach = Lay(TenSach),
                        ChuDe = Lay(ChuDe),
                        NhomSach = Lay(NhomSach),
                        NhaXuatBan = Lay(NhaXuatBan),
                        MoTa = NullNeuRong(Lay(MoTa)),
                        AnhBia = NullNeuRong(Lay(AnhBia)),
                        TacGias = Lay(TacGia)
                            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList()
                    };

                    if (d.TenSach == "") loiDong.Add("thiếu tên sách");
                    else if (d.TenSach.Length > 250) loiDong.Add("tên sách dài quá 250 ký tự");
                    if (d.ChuDe == "") loiDong.Add("thiếu chủ đề");
                    if (d.NhomSach == "") loiDong.Add("thiếu nhóm sách");
                    if (d.NhaXuatBan == "") loiDong.Add("thiếu nhà xuất bản");
                    if (d.AnhBia?.Length > 300) loiDong.Add("đường dẫn ảnh bìa dài quá 300 ký tự");

                    var gia = DocSo(row, viTri, GiaBan);
                    if (gia == null || gia < 0) loiDong.Add($"giá bán \"{Lay(GiaBan)}\" không hợp lệ");
                    else d.GiaBan = gia.Value;

                    var sl = DocSo(row, viTri, SoLuongTon);
                    if (Lay(SoLuongTon) == "") d.SoLuongTon = 0;
                    else if (sl == null || sl < 0 || sl != Math.Floor(sl.Value)) loiDong.Add($"số lượng tồn \"{Lay(SoLuongTon)}\" không hợp lệ");
                    else d.SoLuongTon = (int)sl.Value;

                    var nam = DocSo(row, viTri, NamXB);
                    if (Lay(NamXB) != "")
                    {
                        if (nam == null || nam < 1000 || nam > DateTime.Now.Year + 1 || nam != Math.Floor(nam.Value))
                            loiDong.Add($"năm xuất bản \"{Lay(NamXB)}\" không hợp lệ");
                        else d.NamXB = (int)nam.Value;
                    }

                    var moi = DocCoKhong(Lay(SachMoi));
                    if (moi == null) loiDong.Add($"cột Sách mới phải là Có/Không");
                    else d.SachMoi = moi.Value;

                    var noiBat = DocCoKhong(Lay(NoiBat));
                    if (noiBat == null) loiDong.Add($"cột Nổi bật phải là Có/Không");
                    else d.NoiBat = noiBat.Value;

                    if (loiDong.Count > 0) kq.Loi.Add($"Dòng {r}: {string.Join("; ", loiDong)}.");
                    else kq.Dongs.Add(d);
                }

                if (kq.Dongs.Count == 0 && kq.Loi.Count == 0) kq.Loi.Add("File không có dòng sách nào.");
            }

            return kq;
        }

        private static decimal? DocSo(IXLRow row, Dictionary<string, int> viTri, string cot)
        {
            if (!viTri.TryGetValue(cot, out var c)) return null;
            var cell = row.Cell(c);
            if (cell.IsEmpty()) return null;
            if (cell.DataType == XLDataType.Number) return (decimal)cell.GetDouble();

            // Văn bản như "86.000", "86,000 đ", "86000₫" — tiền VNĐ không có phần lẻ
            var text = new string(cell.GetString().Where(ch => !char.IsWhiteSpace(ch) && ch != '.' && ch != ',').ToArray())
                .Replace("₫", "").Replace("đ", "").Replace("Đ", "").Replace("VND", "", StringComparison.OrdinalIgnoreCase);
            return decimal.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : null;
        }

        private static bool? DocCoKhong(string value)
        {
            var v = ChuanHoa(value);
            if (v is "" or "khong" or "0" or "false" or "no" or "k") return false;
            if (v is "co" or "x" or "1" or "true" or "yes" or "c") return true;
            return null;
        }

        private static string? NullNeuRong(string s) => string.IsNullOrWhiteSpace(s) ? null : s;

        /// <summary>Bỏ dấu tiếng Việt, khoảng trắng thừa và chuyển chữ thường để so khớp.</summary>
        public static string ChuanHoa(string s)
        {
            var normalized = s.Trim().ToLowerInvariant().Replace('đ', 'd').Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark) sb.Append(ch);
            }
            return string.Join(' ', sb.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
