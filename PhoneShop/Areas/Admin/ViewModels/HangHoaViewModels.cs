
using System.ComponentModel.DataAnnotations;

namespace PhoneShop.Areas.Admin.ViewModels

{
    

    public class HangHoaViewModels

    {
        public int MaHh { get; set; }
        [Required(ErrorMessage = "Tên hàng hóa không được để trống")]
        public string TenHh { get; set; } = "";



        [Required(ErrorMessage = "Mã loại là bắt buộc")]
        public int MaLoai { get; set; }



        [Required, Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải là một số dương")]
        public double? DonGia { get; set; }

        public string? Hinh { get; set; }

        [Required, Range(0, 100, ErrorMessage = "Giảm giá phải từ 0 đến 100")]
        public double GiamGia { get; set; }

        [Required, Range(0, int.MaxValue, ErrorMessage = "Số lần xem phải là số nguyên dương")]
        public int SoLanXem { get; set; }

        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Mã nhà cung cấp không được để trống")]
        public string MaNcc { get; set; } = null!;


    }
}


