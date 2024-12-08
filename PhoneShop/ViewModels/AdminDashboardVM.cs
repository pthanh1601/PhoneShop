using PhoneShop.Data;

namespace PhoneShop.ViewModels
{
    public class AdminDashboardVM
    {
        public double TongDoanhThu { get; set; } // Tổng doanh thu trong năm
        public int TongSoHoaDon { get; set; } // Tổng số hóa đơn trong năm
        public int TongSoKhachHang { get; set; } // Tổng số khách hàng
        public List<DoanhThuThangDto> DoanhThuTheoThang { get; set; } = new List<DoanhThuThangDto>(); // Doanh thu từng tháng
        public List<SanPhamBanChayDto> SanPhamBanChay { get; set; } = new List<SanPhamBanChayDto>(); // Top sản phẩm bán chạy
        public List<TopKhachHangDto> TopKhachHang { get; set; } = new List<TopKhachHangDto>(); // Top 5 khách hàng

    }

    // DTO phụ trợ
    public class DoanhThuThangDto
    {
        public int Thang { get; set; }
        public double DoanhThu { get; set; }
    }

    public class SanPhamBanChayDto
    {
        public string TenSanPham { get; set; } = string.Empty;
        public int SoLuongBan { get; set; }
    }

    public class TopKhachHangDto
    {
        public string HoTen { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public int SoHoaDon { get; set; }
        public double TongChiTieu { get; set; }
    }
}