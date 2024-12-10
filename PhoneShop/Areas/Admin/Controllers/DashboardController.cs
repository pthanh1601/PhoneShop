using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using PhoneShop.Data;
using PhoneShop.Helper;
using PhoneShop.ViewModels;
using X.PagedList;
using X.PagedList.Extensions;


namespace MyApp.Namespace
{
    [Area("Admin")]
    [Route("Admin/Dashboard")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly Hshop2023Context _context;
        public DashboardController(Hshop2023Context context)
        {
            _context = context;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            var currentYear = DateTime.Now.Year;

            // Truy vấn toàn bộ hóa đơn trong năm hiện tại
            var hoaDons = _context.HoaDons
                 .Include(h => h.MaKhNavigation) // Eager load khách hàng
                .Include(h => h.ChiTietHds) // Bao gồm chi tiết hóa đơn
                .ThenInclude(ct => ct.MaHhNavigation) // Bao gồm sản phẩm
                .Where(h => h.NgayDat.Year == currentYear)
                .ToList();

            var viewModel = new AdminDashboardVM
            {
                // Tổng doanh thu trong năm
                TongDoanhThu = hoaDons
                    .SelectMany(h => h.ChiTietHds)
                    .Sum(ct => ct.SoLuong * ct.DonGia - ct.GiamGia),

                // Tổng số hóa đơn trong năm
                TongSoHoaDon = hoaDons.Count,
                // Tổng số khách hàng
                TongSoKhachHang = _context.KhachHangs.Count(),
                // Doanh thu theo tháng

                DoanhThuTheoThang = hoaDons
                    .GroupBy(h => h.NgayDat.Month)
                    .Select(g => new DoanhThuThangDto
                    {
                        Thang = g.Key,
                        DoanhThu = g.SelectMany(h => h.ChiTietHds)
                                    .Sum(ct => ct.SoLuong * ct.DonGia - ct.GiamGia)
                    })
                    .OrderBy(x => x.Thang)
                    .ToList(),

                // Sản phẩm bán chạy (Top 5)
                SanPhamBanChay = hoaDons
                    .SelectMany(h => h.ChiTietHds)
                    .GroupBy(ct => ct.MaHhNavigation.TenHh)
                    .OrderByDescending(g => g.Sum(ct => ct.SoLuong))
                    .Take(5)
                    .Select(g => new SanPhamBanChayDto
                    {
                        TenSanPham = g.Key ?? "N/A",
                        SoLuongBan = g.Sum(ct => ct.SoLuong)
                    })
                    .ToList(),



                // Top 5 khách hàng
                TopKhachHang = hoaDons
    .Where(h => h.MaKhNavigation != null) // Loại bỏ hóa đơn không có khách hàng
    .GroupBy(h => h.MaKhNavigation!)
    .OrderByDescending(g => g
        .SelectMany(h => h.ChiTietHds)
        .Sum(ct => ct.SoLuong * ct.DonGia - ct.GiamGia))
    .Take(5)
    .Select(g => new TopKhachHangDto
    {
        HoTen = g.Key?.HoTen ?? "N/A", // Kiểm tra null và đặt giá trị mặc định
        SoDienThoai = g.Key?.DienThoai ?? "N/A",
        SoHoaDon = g.Count(),
        TongChiTieu = g.SelectMany(h => h.ChiTietHds)
                       .Sum(ct => ct.SoLuong * ct.DonGia - ct.GiamGia)
    })
    .ToList()

            };

            return View(viewModel);
        }

        // API lấy doanh thu theo năm
        [HttpGet("GetRevenueByYear")]
        public IActionResult GetRevenueByYear()
        {
            var revenueByYear = _context.HoaDons
                .Where(h => h.NgayDat != null)  // Kiểm tra nếu NgayDat không null
                .GroupBy(h => h.NgayDat.Year)  // Nhóm theo năm
                .Select(g => new
                {
                    Year = g.Key,
                    Revenue = g.Sum(h => h.ChiTietHds.Sum(ct => ct.SoLuong * ct.DonGia - ct.GiamGia))  // Tổng doanh thu của từng năm
                })
                .OrderByDescending(g => g.Year)  // Sắp xếp theo năm giảm dần
                .ToList();

            return Json(revenueByYear);  // Trả về kết quả dưới dạng JSON
        }

        [HttpGet("GetRevenueByMonth")]
        public IActionResult GetRevenueByMonth(int year)
        {
            var revenueByMonth = _context.HoaDons
                .Where(h => h.NgayDat.Year == year) // Lọc theo năm được chọn
                .SelectMany(h => h.ChiTietHds) // Lấy tất cả chi tiết hóa đơn
                .GroupBy(ct => new { ct.MaHdNavigation.NgayDat.Month, ct.MaHdNavigation.NgayDat.Year }) // Nhóm theo tháng và năm
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(ct => ct.SoLuong * ct.DonGia - ct.GiamGia) // Tính tổng doanh thu
                })
                .OrderBy(g => g.Month)  // Sắp xếp theo tháng
                .ToList();

            return Json(revenueByMonth);  // Trả về kết quả dưới dạng JSON
        }


    }
}