﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhoneShop.Data;
using PhoneShop.Areas.Admin.ViewModels;

using X.PagedList;
using X.PagedList.Extensions;

namespace PhoneShop.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/hanghoa")]
    public class HangHoasController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IWebHostEnvironment environment;

        public HangHoasController(Hshop2023Context context, IWebHostEnvironment environment)
        {
            this.db = context;
            this.environment = environment;
        }

        [Route("hanghoa")]
        public IActionResult DanhSachHangHoa(int? page)
        {
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var lstHangHoa = db.HangHoas.AsNoTracking().OrderBy(h => h.TenHh).ToPagedList(pageNumber, pageSize);
            return View(lstHangHoa);
        }

        [Route("ThemHangHoa")]
        [HttpGet]
        public IActionResult ThemHangHoa()
        {
            var nccList = db.NhaCungCaps.ToList();
            ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai");
            ViewBag.MaNcc = new SelectList(nccList, "MaNcc", "TenCongTy");

            return View();
        }

        [Route("ThemHangHoa")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemHangHoa(HangHoaViewModels hanghoa, IFormFile hinh)
        {
            // Kiểm tra nếu dữ liệu không hợp lệ
            if (!ModelState.IsValid)
            {
                // Trả về view cùng với thông báo lỗi để người dùng chỉnh sửa
                var nccList = db.NhaCungCaps.ToList();
                ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai");
                ViewBag.MaNcc = new SelectList(nccList, "MaNcc", "TenCongTy");

                return View(hanghoa);
            }

            // Kiểm tra nếu có tệp hình ảnh được chọn
            if (hinh != null && hinh.Length > 0)
            {
                // Lấy tên tệp và lưu vào thư mục 'images'
                string fileName = Path.GetFileName(hinh.FileName);
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                // Tạo thư mục nếu chưa có
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Tạo đường dẫn để lưu tệp
                string filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu tệp vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    hinh.CopyTo(fileStream);
                }

                // Lưu tên tệp vào cơ sở dữ liệu (chỉ lưu tên tệp, không lưu đường dẫn đầy đủ)
                hanghoa.Hinh = fileName;
            }

            // Nếu không có tệp hình ảnh, giữ lại giá trị cũ cho cột Hinh nếu có
            else
            {
                hanghoa.Hinh = hanghoa.Hinh; // Giữ lại giá trị đã có trong view model nếu không chọn ảnh
            }

            // Tiến hành thêm hàng hóa vào cơ sở dữ liệu
            var themhanghoa = new HangHoa
            {
                TenHh = hanghoa.TenHh,
                MaLoai = hanghoa.MaLoai,
                DonGia = (double)hanghoa.DonGia,
                Hinh = hanghoa.Hinh,
                GiamGia = hanghoa.GiamGia,
                SoLanXem = hanghoa.SoLanXem,
                MoTa = hanghoa.MoTa,
                MaNcc = hanghoa.MaNcc,
            };

            db.HangHoas.Add(themhanghoa);
            db.SaveChanges();

            // Chuyển hướng đến danh sách hàng hóa sau khi thêm
            return RedirectToAction("DanhSachHangHoa");
        }




        //   Sửa hàng hóa
        //[Route("SuaHangHoa")]
        //[HttpGet]
        //      public IActionResult SuaHangHoa(int MaHh)
        //   {

        //       ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai");
        //       ViewBag.MaNcc = new SelectList(db.NhaCungCaps.ToList(), "MaNcc", "TenCongTy");

        //       var hangHoa = db.HangHoas.Find(MaHh);
        //       return View(hangHoa);
        //   }

        //[Route("SuaHangHoa")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult SuaHangHoa(HangHoa hangHoa)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(hangHoa).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("DanhSachHangHoa", "HomeAdmin");
        //    }
        //    return View(hangHoa);


        //}



        //[Route("SuaHangHoa")]
        //[HttpGet]
        //public IActionResult SuaHangHoa(int MaHh)
        //{
        //    //var nccList = db.NhaCungCaps.ToList();
        //    ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai");
        //    ViewBag.MaNcc = new SelectList(db.NhaCungCaps.ToList(), "MaNcc", "TenCongTy");
        //    //ViewBag.MaNcc = new SelectList(nccList, "MaNcc", "TenCongTy");
        //    var hangHoa = db.HangHoas.Find(MaHh);
        //    return View(hangHoa);
        //}

        //[Route("SuaHangHoa")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult SuaHangHoa(HangHoa hangHoa)
        //{
        //    // Kiểm tra MaHh được truyền không
        //    if (hangHoa.MaHh == 0)
        //    {
        //        ModelState.AddModelError(string.Empty, "Không tìm thấy MaHh");
        //    }
        // var existingHangHoa = db.HangHoas.Find(hangHoa.MaHh);

        // // Cập nhật từng thuộc tính để ép kiểu dữ liệu
        // existingHangHoa.TenHh = (string)hangHoa.TenHh;
        // existingHangHoa.MaLoai = (int)hangHoa.MaLoai;
        // existingHangHoa.DonGia = (double)hangHoa.DonGia;
        // existingHangHoa.Hinh = (string)hangHoa.Hinh;

        // existingHangHoa.GiamGia = (double)hangHoa.GiamGia;
        // existingHangHoa.SoLanXem = (int)hangHoa.SoLanXem;
        // existingHangHoa.MoTa = (string)hangHoa.MoTa;
        // existingHangHoa.MaNcc = (string)hangHoa.MaNcc;

        //// Đánh dấu đối tượng là đã thay đổi và lưu thay đổi
        // db.Entry(existingHangHoa).State = EntityState.Modified;
        // db.SaveChanges();
        // return RedirectToAction("DanhSachHangHoa");

        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(hangHoa).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("DanhSachHangHoa");


        //    }


        //    ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai");
        //    ViewBag.MaNcc = new SelectList(db.NhaCungCaps.ToList(), "MaNcc", "TenCongTy");
        //    return View(hangHoa);




        //}











        //[Route("SuaHangHoa")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult SuaHangHoa(HangHoa hangHoa)
        //{

        //    // Tìm đối tượng hàng hóa từ cơ sở dữ liệu
        //    var existingHangHoa = db.HangHoas.Find(hangHoa.MaHh);

        //    // Cập nhật từng thuộc tính để ép kiểu dữ liệu
        //    existingHangHoa.TenHh = (string)hangHoa.TenHh;
        //    existingHangHoa.MaLoai = (int)hangHoa.MaLoai;
        //    existingHangHoa.DonGia = (double)hangHoa.DonGia;
        //    existingHangHoa.Hinh = hangHoa.Hinh;
        //    existingHangHoa.GiamGia = (float)hangHoa.GiamGia;
        //    existingHangHoa.SoLanXem = (int)hangHoa.SoLanXem;
        //    existingHangHoa.MoTa = (string)hangHoa.MoTa;
        //    existingHangHoa.MaNcc = (string)hangHoa.MaNcc;

        //    // Đánh dấu đối tượng là đã thay đổi và lưu thay đổi
        //    db.Entry(existingHangHoa).State = EntityState.Modified;
        //    db.SaveChanges();
        //    return RedirectToAction("DanhSachHangHoa");




        //    // Nạp lại các ViewBag cho dropdown trong trường hợp ModelState không hợp lệ
        //    ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai");
        //    ViewBag.MaNcc = new SelectList(db.NhaCungCaps.ToList(), "MaNcc", "TenCongTy");

        //    return View(hangHoa);
        //}














        //Xóa hàng hóa

        [Route("XoaHangHoa")]
        [HttpGet]
        public IActionResult XoaHangHoa(int MaHh)
        {
            TempData["Message"] = "";

            // Kiểm tra xem hàng hóa có tồn tại trong bảng ChiTietHds không
            var chiTietHD = db.ChiTietHds.Where(x => x.MaHh == MaHh);
            if (chiTietHD.Any())
            {
                TempData["Message"] = "Không xóa được hàng hóa này vì có chi tiết hóa đơn liên quan.";
                return RedirectToAction("DanhSachHangHoa");
            }

            // Tìm hàng hóa trong cơ sở dữ liệu
            var hangHoa = db.HangHoas.Find(MaHh);
            if (hangHoa == null)
            {
                TempData["Message"] = "Hàng hóa không tồn tại.";
                return RedirectToAction("DanhSachHangHoa");
            }

            // Xóa hàng hóa
            db.HangHoas.Remove(hangHoa);
            db.SaveChanges();
            TempData["Message"] = "Hàng hóa đã được xóa thành công.";

            return RedirectToAction("DanhSachHangHoa");
        }




        //sửa hàng hóa
        [Route("SuaHangHoa")]
        [HttpGet]
        public IActionResult SuaHangHoa(int MaHh)
        {
            var hangHoa = db.HangHoas.Find(MaHh);

            if (hangHoa == null)
            {
                return RedirectToAction("DanhSachHangHoa");
            }
            var hanghoa = new HangHoaViewModels()
            {
                TenHh = hangHoa.TenHh,
                MaLoai = (int)hangHoa.MaLoai,
                DonGia = hangHoa.DonGia,
                Hinh = hangHoa.Hinh,
                GiamGia = (double)hangHoa.GiamGia,
                SoLanXem = (int)hangHoa.SoLanXem,
                MoTa = hangHoa.MoTa,
                MaNcc = hangHoa.MaNcc,

            };
            ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai");
            ViewBag.MaNcc = new SelectList(db.NhaCungCaps.ToList(), "MaNcc", "TenCongTy");



            ViewData["MaHh"] = hangHoa.MaHh;
            ViewData["NgaySX"] = hangHoa.NgaySx;
            return View(hanghoa);

        }
        [Route("SuaHangHoa")]
        [HttpPost]
        public IActionResult SuaHangHoa(int MaHh, HangHoaViewModels hanghoa, IFormFile Hinh)
        {
            var hangHoa = db.HangHoas.Find(MaHh);

            if (hangHoa == null)
            {
                return RedirectToAction("DanhSachHangHoa");
            }

            if (!ModelState.IsValid)
            {
                ViewData["MaHh"] = hangHoa.MaHh;
                ViewData["NgaySX"] = hangHoa.NgaySx;

                return View(hanghoa);
            }

            hangHoa.TenHh = hanghoa.TenHh;
            hangHoa.MaLoai = hanghoa.MaLoai;
            hangHoa.DonGia = hanghoa.DonGia;
            hangHoa.GiamGia = hanghoa.GiamGia;
            hangHoa.SoLanXem = hanghoa.SoLanXem;
            hangHoa.MoTa = hanghoa.MoTa;
            hangHoa.MaNcc = hanghoa.MaNcc;

            if (Hinh != null && Hinh.Length > 0)
            {
                var fileName = Path.GetFileName(Hinh.FileName);  // Lấy tên tệp hình ảnh

                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "HangHoa");

                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                var filePath = Path.Combine(uploadDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Hinh.CopyTo(stream);
                }

                hangHoa.Hinh = fileName;  // Cập nhật tên tệp hình ảnh vào cơ sở dữ liệu
            }

            db.SaveChanges();
            db.Entry(hangHoa).Reload();

            return RedirectToAction("DanhSachHangHoa");
        }






    }
}
