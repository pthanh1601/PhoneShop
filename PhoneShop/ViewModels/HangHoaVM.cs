namespace PhoneShop.ViewModels
{
    public class HangHoaVM
    {
        public int MaHh { get; set; }

        public string TenHh { get; set; }

        public string? Hinh { get; set; } // Cho phép null để không bắt buộc ảnh mới

        public string? HinhCu { get; set; } // Lưu tên ảnh cũ khi không chọn ảnh mới

        public double DonGia { get; set; }

        public string? MotaNgan { get; set; } // Cho phép null nếu không bắt buộc

        public string? TenLoai { get; set; } // Cho phép null nếu không bắt buộc
    }
}
