using PhoneShop.Data;

namespace PhoneShop.ViewModels
{
    public class PhanCongVM
    {
        public PhanCong PhanCong { get; set; }
        public List<NhanVien> NhanViens { get; set; } // Danh sách nhân viên
        public List<PhongBan> PhongBans { get; set; } // Danh sách phòng ban
    }
}
