using Microsoft.AspNetCore.Mvc;
using PhoneShop.Data;
using PhoneShop.Models; // Import namespace chứa model của bạn
using PhoneShop.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneShop.Controllers
{
    public class HangHoas1Controller : Controller
    {
        private readonly Hshop2023Context db;

        public HangHoas1Controller(Hshop2023Context context)
        {
            db = context;
        }

        [HttpGet]
        // Controller - GetProducts
        [HttpPost]
        public async Task<IActionResult> GetProducts(int? loai, int page = 1)
        {
            int pageSize = 3; // Số sản phẩm mỗi trang

            var hangHoas = db.HangHoas.AsQueryable();

            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }

            // Chọn các trường cần thiết
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHh = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MotaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });

            var paginatedList = await PaginatedList<HangHoaVM>.CreateAsync(result, page, pageSize);

            return Json(new
            {
                data = paginatedList,
                totalPages = paginatedList.TotalPages,
                currentPage = page,
                totalCount = paginatedList.TotalCount
            });
        }

    }
}
