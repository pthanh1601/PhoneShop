using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneShop.Data;
using PhoneShop.Helper;
using PhoneShop.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using AutoMapper;

namespace PhoneShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/KhachHang")]
    public class KhachHangController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;

        public KhachHangController(Hshop2023Context context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        #region Danh Sách Khách Hàng
        [Route("")]
        [Route("DanhSachKhachHang")]
        public IActionResult DanhSachKhachHang()
        {
            var danhSachKhachHang = db.KhachHangs.ToList();
            return View("~/Areas/Admin/Views/KhachHang/DanhSachKhachHang.cshtml", danhSachKhachHang);
        }
        #endregion

        #region Create KhachHang
        [HttpGet("Create")]
        public IActionResult ThemKhachHang()
        {
            return View("~/Areas/Admin/Views/KhachHang/ThemKhachHang.cshtml");
        }

        [HttpPost("Create")]
        public IActionResult ThemKhachHang(RegisterVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = new KhachHang
                    {
                        MaKh = model.MaKh,
                        HoTen = model.HoTen,
                        MatKhau = model.MatKhau.ToMd5Hash(MyUtil.GenerateRandomKey()),
                        Email = model.Email,
                        DiaChi = model.DiaChi,
                        DienThoai = model.DienThoai,
                        GioiTinh = model.GioiTinh,
                        NgaySinh = (DateTime)model.NgaySinh,
                        HieuLuc = true,
                        VaiTro = 0
                    };

                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }

                    db.KhachHangs.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction(nameof(DanhSachKhachHang));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while adding the customer: " + ex.Message);
                }
            }

            return View("~/Areas/Admin/Views/KhachHang/ThemKhachHang.cshtml", model);
        }
        #endregion

        #region Edit KhachHang
        [HttpGet("Edit/{id}")]
        public IActionResult SuaKhachHang(string id)
        {
            var khachHang = db.KhachHangs.Find(id);
            if (khachHang == null) return NotFound();

            return View("~/Areas/Admin/Views/KhachHang/SuaKhachHang.cshtml", khachHang);
        }

        [HttpPost("Edit/{id}")]
        public IActionResult SuaKhachHang(KhachHang model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.Find(model.MaKh);
                if (khachHang == null) return NotFound();

                khachHang.HoTen = model.HoTen;
                khachHang.Email = model.Email;
                khachHang.DiaChi = model.DiaChi;
                khachHang.DienThoai = model.DienThoai;
                khachHang.GioiTinh = model.GioiTinh;
                khachHang.NgaySinh = model.NgaySinh;

                if (Hinh != null)
                {
                    khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                }

                db.SaveChanges();
                return RedirectToAction(nameof(DanhSachKhachHang));
            }

            return View("~/Areas/Admin/Views/KhachHang/SuaKhachHang.cshtml", model);
        }
        #endregion

        #region Delete KhachHang
        [HttpGet("Delete/{id}")]
        public IActionResult XoaKhachHang(string id)
        {
            var khachHang = db.KhachHangs.Find(id);
            if (khachHang == null) return NotFound();
            return View("~/Areas/Admin/Views/KhachHang/XoaKhachHang.cshtml", khachHang);
        }

        [HttpPost("Delete/{id}")]
        public IActionResult XacNhanXoa(string id)
        {
            var khachHang = db.KhachHangs.Find(id);
            if (khachHang != null)
            {
                db.KhachHangs.Remove(khachHang);
                db.SaveChanges();
            }
            return RedirectToAction(nameof(DanhSachKhachHang));
        }
        #endregion
    }
}
