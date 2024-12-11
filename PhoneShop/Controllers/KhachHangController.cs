using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PhoneShop.Data;
using PhoneShop.Helper;
using PhoneShop.ViewModels;
using System.Security.Claims;
namespace PhoneShop.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;
        public KhachHangController(Hshop2023Context context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }


        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            // Lấy MaKh hoặc Email từ Claims
            var maKh = User.FindFirst(MySetting.CLAIM_CUSTOMERID)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(maKh) && string.IsNullOrEmpty(email))
            {
                // Không tìm thấy thông tin, chuyển hướng tới trang lỗi hoặc đăng nhập
                return RedirectToAction("Login");
            }

            // Truy vấn cơ sở dữ liệu dựa trên MaKh hoặc Email
            KhachHang khachHang;
            if (!string.IsNullOrEmpty(maKh))
            {
                khachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKh == maKh);
            }
            else
            {
                khachHang = db.KhachHangs.FirstOrDefault(kh => kh.Email == email);
            }

            if (khachHang == null)
            {
                // Nếu không tìm thấy khách hàng
                return RedirectToAction("404");
            }

            // Ánh xạ từ KhachHang sang RegisterVM
            var registerVM = new RegisterVM
            {
                MaKh = khachHang.MaKh,
                MatKhau = khachHang.MatKhau, // Bạn có thể ẩn thuộc tính này nếu không cần hiển thị
                HoTen = khachHang.HoTen,
                GioiTinh = khachHang.GioiTinh,
                NgaySinh = khachHang.NgaySinh,
                DiaChi = khachHang.DiaChi,
                DienThoai = khachHang.DienThoai,
                Email = khachHang.Email,
                Hinh = khachHang.Hinh
            };

            return View(registerVM);
        }


        [Authorize]
        [HttpPost]
        public IActionResult Update(RegisterVM model, string? CurrentPassword, string? NewPassword, string? RepeatPassword, IFormFile? Hinh)
        {
            // Kiểm tra mật khẩu
            var khachHang = db.KhachHangs.FirstOrDefault(x => x.MaKh == model.MaKh);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Khách hàng không tồn tại." });
            }

            if (!string.IsNullOrEmpty(CurrentPassword) && !string.IsNullOrEmpty(NewPassword))
            {
                var hashedPassword = CurrentPassword.ToMd5Hash(khachHang.RandomKey);
                if (khachHang.MatKhau != hashedPassword)
                {
                    return Json(new { success = false, message = "Mật khẩu hiện tại không chính xác." });
                }

                if (NewPassword != RepeatPassword)
                {
                    return Json(new { success = false, message = "Mật khẩu mới không khớp." });
                }

                // Cập nhật mật khẩu mới
                khachHang.RandomKey = MyUtil.GenerateRandomKey();
                khachHang.MatKhau = NewPassword.ToMd5Hash(khachHang.RandomKey);
            }

            // Cập nhật các trường khác
            khachHang.HoTen = model.HoTen;
            khachHang.Email = model.Email;
            khachHang.DiaChi = model.DiaChi;
            khachHang.NgaySinh = model.NgaySinh ?? DateTime.Now;
            khachHang.GioiTinh = model.GioiTinh;
            khachHang.DienThoai = model.DienThoai;

            // Xử lý upload hình ảnh
            if (Hinh != null)
            {
                khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
            }

            db.SaveChanges();
            return Json(new { success = true });
        }



        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
