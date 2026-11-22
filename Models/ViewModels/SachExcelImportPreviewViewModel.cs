using BanSach.Models;

namespace BanSach.Models.ViewModels
{
    public class SachExcelImportPreviewViewModel
    {
        public string? Token { get; set; }
        public List<string> FileErrors { get; set; } = new();
        public List<SachExcelImportRowViewModel> Rows { get; set; } = new();

        public bool CanConfirm =>
            FileErrors.Count == 0 &&
            Rows.Count > 0 &&
            Rows.All(x => x.Errors.Count == 0);
    }

    public class SachExcelImportRowViewModel
    {
        public int RowNumber { get; set; }
        public string TenSach { get; set; } = string.Empty;
        public int? MaCD { get; set; }
        public int? MaNhom { get; set; }
        public int? MaNXB { get; set; }
        public decimal? GiaBan { get; set; }
        public int? SoLuongTon { get; set; }
        public string? AnhBia { get; set; }
        public string? MoTa { get; set; }
        public int? NamXB { get; set; }
        public bool SachMoi { get; set; }
        public bool NoiBat { get; set; }
        public List<int> MaTacGias { get; set; } = new();
        public List<string> Errors { get; set; } = new();

        public Sach ToEntity()
        {
            return new Sach
            {
                TenSach = TenSach.Trim(),
                MaCD = MaCD!.Value,
                MaNhom = MaNhom!.Value,
                MaNXB = MaNXB!.Value,
                GiaBan = GiaBan!.Value,
                SoLuongTon = SoLuongTon!.Value,
                AnhBia = string.IsNullOrWhiteSpace(AnhBia) ? null : AnhBia.Trim(),
                MoTa = string.IsNullOrWhiteSpace(MoTa) ? null : MoTa.Trim(),
                NamXB = NamXB,
                SachMoi = SachMoi,
                NoiBat = NoiBat,
                SachTacGias = MaTacGias
                    .Distinct()
                    .Select(maTG => new SachTacGia { MaTG = maTG })
                    .ToList()
            };
        }
    }
}
