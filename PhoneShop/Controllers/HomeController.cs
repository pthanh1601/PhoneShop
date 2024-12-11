using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;  // Đảm bảo đã import thư viện này
using PhoneShop.Data;
using PhoneShop.Models;
using PhoneShop.ViewModels;
using System;
using System.Diagnostics;

namespace PhoneShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(Hshop2023Context context, ILogger<HomeController> logger, IConfiguration configuration)
        {
            db = context;
            _logger = logger;
            _configuration = configuration;
        }

     

        public IActionResult Index(int? loai)
        {
            var hangHoas = db.HangHoas.AsQueryable();
            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHh = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MotaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });


            return View(result);
        }

        [Route("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Hàm kiểm tra kết nối
        public IActionResult CheckConnection()
        {
            try
            {
                // Lấy chuỗi kết nối từ appsettings.json
                var connectionString = _configuration.GetConnectionString("PhoneShop");

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();  // Mở kết nối

                    // Nếu kết nối thành công, trả về thông báo
                    return Content("Kết nối SQL Server thành công.");
                }
            }
            catch (Exception ex)
            {
                // Nếu có lỗi kết nối, trả về thông báo lỗi
                return Content($"Lỗi kết nối: {ex.Message}");
            }
        }
    }
}
