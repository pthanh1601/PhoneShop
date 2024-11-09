using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhoneShop.Data;

public partial class HangHoa
{
    public int MaHh { get; set; }

    [Required(ErrorMessage = "Tên hàng hóa không được để trống")]
    public string TenHh { get; set; } = "";

    public string? TenAlias { get; set; } = "";

    [Required(ErrorMessage = "Mã loại là bắt buộc")]
    public int MaLoai { get; set; } 

    public string? MoTaDonVi { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải là một số dương")]
    public double? DonGia { get; set; }

    public string? Hinh { get; set; }

    public DateTime? NgaySx { get; set; } =null;

    [Range(0, 100, ErrorMessage = "Giảm giá phải từ 0 đến 100")]
    public double GiamGia { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số lần xem phải là số nguyên dương")]
    public int SoLanXem { get; set; }

    public string? MoTa { get; set; }

    [Required(ErrorMessage = "Mã nhà cung cấp không được để trống")]
    public string MaNcc { get; set; } = null!;

    public virtual ICollection<BanBe> BanBes { get; set; } = new List<BanBe>();

    public virtual ICollection<ChiTietHd> ChiTietHds { get; set; } = new List<ChiTietHd>();

    public virtual Loai MaLoaiNavigation { get; set; } = null!;

    public virtual NhaCungCap MaNccNavigation { get; set; } = null!;

    public virtual ICollection<YeuThich> YeuThiches { get; set; } = new List<YeuThich>();
}
