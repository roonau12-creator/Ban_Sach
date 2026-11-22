using ClosedXML.Excel;

namespace BanSach.Helpers
{
    /// <summary>Một dòng chủ đề đọc được từ file Excel.</summary>
    public class DongChuDeExcel
    {
        public int Dong { get; set; }
        public string TenChuDe { get; set; } = string.Empty;
    }

    public class KetQuaDocChuDeExcel
    {
        public List<DongChuDeExcel> Dongs { get; } = new();
        public List<string> Loi { get; } = new();
    }

    public static class ChuDeExcelHelper
    {
        public const string TenChuDe = "Tên chủ đề";
        public const int DoDaiToiDa = 100;
        private const string TenSheet = "ChuDe";

        public static byte[] TaoFileMau(IEnumerable<string> chuDeHienCo)
        {
            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet(TenSheet);

            var header = ws.Cell(1, 1);
            header.Value = TenChuDe + " *";
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.FromHtml("#8b3a2a");
            header.Style.Font.FontColor = XLColor.White;

            var viDu = ws.Cell(2, 1);
            viDu.Value = "Văn học thiếu nhi";
            viDu.Style.Font.Italic = true;
            viDu.Style.Font.FontColor = XLColor.Gray;

            ws.SheetView.FreezeRows(1);
            ws.Column(1).Width = 45;

            var hd = wb.AddWorksheet("Huong dan");
            string[] huongDan =
            {
                "HƯỚNG DẪN NHẬP CHỦ ĐỀ",
                "• Nhập mỗi chủ đề trên một dòng của sheet \"ChuDe\", bắt đầu từ dòng 2 (xóa dòng ví dụ trước khi nhập).",
                $"• Tên chủ đề không được để trống, tối đa {DoDaiToiDa} ký tự.",
                "• Chủ đề trùng tên với chủ đề đã có (không phân biệt hoa thường, dấu) sẽ được bỏ qua hoặc báo lỗi tùy lựa chọn khi nhập."
            };
            var dong = 1;
            foreach (var line in huongDan) hd.Cell(dong++, 1).Value = line;
            hd.Cell(1, 1).Style.Font.Bold = true;

            dong++;
            hd.Cell(dong, 1).Value = "Chủ đề hiện có";
            hd.Cell(dong, 1).Style.Font.Bold = true;
            foreach (var ten in chuDeHienCo) hd.Cell(++dong, 1).Value = ten;
            hd.Column(1).Width = 60;

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        public static KetQuaDocChuDeExcel DocFile(Stream stream)
        {
            var kq = new KetQuaDocChuDeExcel();
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
                var ws = wb.Worksheets.FirstOrDefault(w => w.Name == TenSheet) ?? wb.Worksheet(1);
                var headerRow = ws.FirstRowUsed();
                if (headerRow == null)
                {
                    kq.Loi.Add("File không có dữ liệu.");
                    return kq;
                }

                var cot = headerRow.CellsUsed()
                    .FirstOrDefault(c => SachExcelHelper.ChuanHoa(c.GetString().Replace("*", "")) == SachExcelHelper.ChuanHoa(TenChuDe))
                    ?.Address.ColumnNumber;
                if (cot == null)
                {
                    kq.Loi.Add($"Thiếu cột bắt buộc: {TenChuDe}. Hãy dùng file mẫu.");
                    return kq;
                }

                var lastRow = ws.LastRowUsed()!.RowNumber();
                for (var r = headerRow.RowNumber() + 1; r <= lastRow; r++)
                {
                    var ten = string.Join(' ', ws.Cell(r, cot.Value).GetFormattedString()
                        .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
                    if (ten == "") continue; // bỏ dòng trống

                    if (ten.Length > DoDaiToiDa)
                        kq.Loi.Add($"Dòng {r}: tên chủ đề dài quá {DoDaiToiDa} ký tự.");
                    else
                        kq.Dongs.Add(new DongChuDeExcel { Dong = r, TenChuDe = ten });
                }

                if (kq.Dongs.Count == 0 && kq.Loi.Count == 0) kq.Loi.Add("File không có dòng chủ đề nào.");
            }

            return kq;
        }
    }
}
