using PhoneShop.Data;

namespace PhoneShop.ViewModels
{
    public class HoaDonViewModel
    {
        public int MaHd { get; set; }
        public string MaKh { get; set; } = null!;
        public DateTime NgayDat { get; set; }
        public DateTime? NgayCan { get; set; }
        public DateTime? NgayGiao { get; set; }
        public string? HoTen { get; set; }
        public string DiaChi { get; set; } = null!;
        public string? DienThoai { get; set; }
        public string CachThanhToan { get; set; } = null!;
        public string CachVanChuyen { get; set; } = null!;
        public double PhiVanChuyen { get; set; }
        public int MaTrangThai { get; set; }
        public string? MaNv { get; set; }
        public string? GhiChu { get; set; }

        // Các danh sách để hiển thị trong View
        public List<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();
        public List<HangHoa> HangHoas { get; set; } = new List<HangHoa>();
        public List<ChiTietHd> ChiTietHds { get; set; } = new List<ChiTietHd>();
        public class ChiTietHoaDonViewModel
    {
        public int MaHh { get; set; }
        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public double GiamGia { get; set; }

        // Validation
        public bool IsValid()
        {
            return SoLuong > 0 && DonGia > 0;
        }
    }
    }
}
