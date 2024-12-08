
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhoneShop.Data;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Authorization;
using PhoneShop.Models;


namespace PhoneShop.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        Hshop2023Context db = new Hshop2023Context();
        public HomeAdminController(Hshop2023Context context)
        {
            db = context;
        }
        [Authorize]

        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            // Điều hướng tới trang Dashboard
            return RedirectToAction("Dashboard", "Admin");
        }













        // Ở dưới là danh mục plese tách ra riêng//
        [Route("danhmucsanpham")]
        public IActionResult DanhMucSanPham(int? page)
        {
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var lstDanhMucSanPham = db.Loais.AsNoTracking().OrderBy(x => x.TenLoai).ToPagedList(pageNumber, pageSize);
            return View(lstDanhMucSanPham);
        }

        [Route("ThemDanhMucSanPham")]
        [HttpGet]

        public IActionResult ThemDanhMucSanPham()
        {
            //var nccList = db.NhaCungCaps.ToList();
            //ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai");
            //ViewBag.MaNcc = new SelectList(nccList, "MaNcc", "TenCongTy");

            return View();
        }

        [Route("ThemDanhMucSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemDanhMucSanPham(Loai loai)
        {
            if (ModelState.IsValid)
            {
                db.Loais.Add(loai);
                db.SaveChanges();
                return RedirectToAction("DanhMucSanPham");
            }

            return View(loai);

        }

        //[Route("SuaDanhMucSanPham")]
        //[HttpGet]

        //public IActionResult SuaDanhMucSanPham(int MaLoai)
        //{
        //    //var nccList = db.NhaCungCaps.ToList();
        //    //ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai");
        //    //ViewBag.MaNcc = new SelectList(nccList, "MaNcc", "TenCongTy");
        //    var loai = db.Loais.Find(MaLoai);
        //    return View(loai);
        //}

        //[Route("SuaDanhMucSanPham")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult SuaDanhMucSanPham(Loai loai)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(loai).State= EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("danhmucsanpham","admin");
        //    }

        //    return View(loai);

        //}


        [Route("SuaDanhMucSanPham")]
        [HttpGet]
        public IActionResult SuaDanhMucSanPham(int MaLoai)
        {
            var loai = db.Loais.Find(MaLoai);
            if (loai == null)
            {
                TempData["Message"] = "Danh mục sản phẩm không tồn tại.";
                return RedirectToAction("DanhMucSanPham");
            }

            return View(loai);
        }

        [Route("SuaDanhMucSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaDanhMucSanPham(Loai loai)
        {
            if (ModelState.IsValid)
            {
                var existingLoai = db.Loais.Find(loai.MaLoai);
                if (existingLoai == null)
                {
                    TempData["Message"] = "Danh mục sản phẩm không tồn tại.";
                    return RedirectToAction("DanhMucSanPham", "HomeAdmin");
                }

                // Cập nhật thông tin
                existingLoai.TenLoai = loai.TenLoai;
                existingLoai.TenLoaiAlias = loai.TenLoaiAlias;
                existingLoai.MoTa = loai.MoTa;
                existingLoai.Hinh = loai.Hinh;

                db.Entry(existingLoai).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Message"] = "Danh mục sản phẩm đã được cập nhật thành công.";
                return RedirectToAction("DanhMucSanPham", "HomeAdmin");
            }

            return View(loai);



        }

        [Route("XoaDanhMucSanPham")]
        [HttpGet]
        public IActionResult XoaDanhMucSanPham(int MaLoai)
        {
            db.Remove(db.Loais.Find(MaLoai));
            db.SaveChanges();
            TempData["Message"] = "Danh mục đã được xóa";
            return RedirectToAction("DanhMucSanPham", "HomeAdmin");
        }





    }
}
