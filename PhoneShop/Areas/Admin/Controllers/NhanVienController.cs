using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PhoneShop.Data;
using System;
using System.Linq;

namespace PhoneShop.Areas.Admin.Controllers
{
    [Area("Admin")]
     [Authorize]
    public class NhanVienController : Controller
    {
        private readonly Hshop2023Context _context;
        private readonly IConfiguration _configuration;

        public NhanVienController(Hshop2023Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Hàm kiểm tra kết nối SQL Server
        public IActionResult CheckConnection()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("PhoneShop");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();  // Mở kết nối
                    return Content("Kết nối SQL Server thành công.");
                }
            }
            catch (Exception ex)
            {
                return Content($"Lỗi kết nối: {ex.Message}");
            }
        }

        // Phương thức GET để hiển thị trang thêm nhân viên
        public IActionResult Create()
        {
            return View("~/Areas/Admin/Views/HomeAdmin/ThemNhanVien.cshtml");
        }

        // Phương thức POST để thêm nhân viên vào cơ sở dữ liệu
        [HttpPost]
        public IActionResult Create(NhanVien nhanVien)
        {
            // Kiểm tra nếu email đã tồn tại
            if (_context.NhanViens.Any(nv => nv.Email == nhanVien.Email))
            {
                ModelState.AddModelError("Email", "Email này đã tồn tại.");
            }

            // Kiểm tra nếu mã nhân viên đã tồn tại
            if (_context.NhanViens.Any(nv => nv.MaNv == nhanVien.MaNv))
            {
                ModelState.AddModelError("MaNv", "Mã nhân viên này đã tồn tại.");
            }

            // Kiểm tra nếu ModelState hợp lệ (không có lỗi)
            if (ModelState.IsValid)
            {
                // Thêm nhân viên vào cơ sở dữ liệu
                _context.NhanViens.Add(nhanVien);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));  // Quay lại trang danh sách sau khi thêm thành công
            }

            // Nếu có lỗi, trả lại trang Create với thông báo lỗi
            return View("~/Areas/Admin/Views/HomeAdmin/ThemNhanVien.cshtml", nhanVien);
        }

        // Phương thức GET để hiển thị danh sách nhân viên
        [HttpPost]
        public IActionResult Edit(string MaNv)
        {
            var nhanVien = _context.NhanViens.FirstOrDefault(nv => nv.MaNv == MaNv);  // Tìm nhân viên theo mã
            if (nhanVien == null)
            {
                return NotFound();  // Nếu không tìm thấy nhân viên
            }

            return View("~/Areas/Admin/Views/HomeAdmin/SuaNhanVien.cshtml", nhanVien);  // Trả về trang sửa với dữ liệu nhân viên
        }
        [HttpPost]
        public IActionResult Update(NhanVien nhanVien)
        {
            // Kiểm tra xem có nhân viên nào với email đã tồn tại chưa
            if (_context.NhanViens.Any(nv => nv.Email == nhanVien.Email && nv.MaNv != nhanVien.MaNv))
            {
                ModelState.AddModelError("Email", "Email này đã tồn tại.");
            }

            // Kiểm tra xem mã nhân viên có bị trùng không
            if (_context.NhanViens.Any(nv => nv.MaNv == nhanVien.MaNv && nv.MaNv != nhanVien.MaNv))
            {
                ModelState.AddModelError("MaNv", "Mã nhân viên này đã tồn tại.");
            }

            // Kiểm tra xem dữ liệu có hợp lệ không
            if (ModelState.IsValid)
            {
                // Tìm nhân viên trong cơ sở dữ liệu theo MaNv
                var existingNhanVien = _context.NhanViens.FirstOrDefault(nv => nv.MaNv == nhanVien.MaNv);
                if (existingNhanVien != null)
                {
                    // Cập nhật các trường thông tin của nhân viên
                    existingNhanVien.HoTen = nhanVien.HoTen;
                    existingNhanVien.Email = nhanVien.Email;
                    existingNhanVien.MatKhau = nhanVien.MatKhau;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.NhanViens.Update(existingNhanVien);
                    _context.SaveChanges();

                    // Chuyển hướng về trang danh sách nhân viên
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Nếu không tìm thấy nhân viên, có thể thông báo lỗi
                    return NotFound();
                }
            }

            // Nếu dữ liệu không hợp lệ, trả lại trang chỉnh sửa
            return View("~/Areas/Admin/Views/HomeAdmin/SuaNhanVien.cshtml", nhanVien);
        }
        [HttpPost]
        public IActionResult Delete(string maNv)
        {
            // Tìm nhân viên trong cơ sở dữ liệu theo MaNv
            var nhanVien = _context.NhanViens.FirstOrDefault(nv => nv.MaNv == maNv);

            if (nhanVien != null)
            {
                // Xóa nhân viên
                _context.NhanViens.Remove(nhanVien);
                _context.SaveChanges();

                // Chuyển hướng về trang danh sách nhân viên
                return RedirectToAction(nameof(Index));
            }

            // Nếu không tìm thấy nhân viên, trả về lỗi
            return NotFound();
        }

        public IActionResult Index()
        {
            // Truy vấn danh sách nhân viên từ cơ sở dữ liệu
            var employees = _context.NhanViens.ToList();
            return View("~/Areas/Admin/Views/HomeAdmin/NhanVien.cshtml", employees);
        }
    }
}
