using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;  // Đảm bảo đã import thư viện này
using PhoneShop.Models;
using System;
using System.Diagnostics;

namespace PhoneShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
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
