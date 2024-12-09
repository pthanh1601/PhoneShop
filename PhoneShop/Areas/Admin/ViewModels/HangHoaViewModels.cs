using System.ComponentModel.DataAnnotations;

namespace PhoneShop.Areas.Admin.ViewModels
{
    public class HangHoaViewModels
    {
        public int MaHh { get; set; }  // Mã hàng hóa

        [Required(ErrorMessage = "Tên hàng hóa không được để trống")]
        public string TenHh { get; set; } = "";  // Tên hàng hóa

        [Required(ErrorMessage = "Mã loại là bắt buộc")]
        public int MaLoai { get; set; }  // Mã loại

        [Required, Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải là một số dương")]
        public double? DonGia { get; set; }  // Đơn giá

        public string? Hinh { get; set; }  // Hình ảnh (nullable, có thể null nếu không có ảnh mới)

        [Required, Range(0, 100, ErrorMessage = "Giảm giá phải từ 0 đến 100")]
        public double GiamGia { get; set; }  // Giảm giá

        [Required, Range(0, int.MaxValue, ErrorMessage = "Số lần xem phải là số nguyên dương")]
        public int SoLanXem { get; set; }  // Số lần xem

        public string? MoTa { get; set; }  // Mô tả sản phẩm (nullable)

        [Required(ErrorMessage = "Mã nhà cung cấp không được để trống")]
        public string MaNcc { get; set; } = null!;  // Mã nhà cung cấp (bắt buộc)

        // Thuộc tính mới cho phép giữ tên ảnh cũ khi không chọn ảnh mới
        public string? HinhCu { get; set; }  // Hình ảnh cũ (nullable)
    }
}
