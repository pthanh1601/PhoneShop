using PhoneShop.Data;

namespace PhoneShop.ViewModels
{
    public class CreateHoaDon()
    {
   public HoaDon HoaDon { get; set; } = new HoaDon();
    public List<HangHoa> HangHoas { get; set; } = new List<HangHoa>();
    public List<TrangThai> TrangThais { get; set; } = new List<TrangThai>();
    public List<ChiTietHd> ChiTietHds { get; set; } = new List<ChiTietHd>();
    public string? SearchKeyword { get; set; } // Từ khóa tìm kiếm
    public int? FilterCategoryId { get; set; } // Bộ lọc loại hàng hóa
    }
}