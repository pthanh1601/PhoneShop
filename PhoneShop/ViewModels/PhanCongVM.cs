using PhoneShop.Data;

namespace PhoneShop.ViewModels
{
    public class PhanCongVM
    {
        public PhanCong PhanCong { get; set; }

        // Danh sách nhân viên và phòng ban không bao giờ null
        public List<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
        public List<PhongBan> PhongBans { get; set; } = new List<PhongBan>();
    }

}
