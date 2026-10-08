namespace BanSach.Models
{
    public class SachTacGia
    {
        public int MaSach { get; set; }
        public int MaTG { get; set; }

        public Sach? Sach { get; set; }
        public TacGia? TacGia { get; set; }
    }
}
