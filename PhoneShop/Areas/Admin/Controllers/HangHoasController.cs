using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhoneShop.Data;
using PhoneShop.Areas.Admin.ViewModels;

using X.PagedList;
using X.PagedList.Extensions;
using PhoneShop.ViewModels;

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
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "HangHoa");

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
                SoLanXem = 0,
                MoTa = hanghoa.MoTa,
                MaNcc = hanghoa.MaNcc,
            };

            db.HangHoas.Add(themhanghoa);
            db.SaveChanges();

            // Chuyển hướng đến danh sách hàng hóa sau khi thêm
            return RedirectToAction("DanhSachHangHoa");
        }



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




        [Route("SuaHangHoa")]
        [HttpGet]
        public IActionResult SuaHangHoa(int MaHh)
        {
            // Tìm hàng hóa theo mã
            var hangHoa = db.HangHoas.Find(MaHh);

            if (hangHoa == null)
            {
                // Nếu không tìm thấy, chuyển hướng về danh sách hàng hóa
                return RedirectToAction("DanhSachHangHoa");
            }

            // Tạo đối tượng ViewModel để truyền dữ liệu ra view
            var hanghoa = new HangHoaViewModels()
            {
                MaHh = hangHoa.MaHh, // Thêm MaHh vào ViewModel
                TenHh = hangHoa.TenHh,
                MaLoai = (int)hangHoa.MaLoai,
                DonGia = hangHoa.DonGia,
                Hinh = hangHoa.Hinh,
                GiamGia = hangHoa.GiamGia.HasValue ? (double)hangHoa.GiamGia : 0, // Kiểm tra null
                SoLanXem = hangHoa.SoLanXem.HasValue ? (int)hangHoa.SoLanXem : 0, // Kiểm tra null
                MoTa = hangHoa.MoTa,
                MaNcc = hangHoa.MaNcc,
            };

            // Truyền danh sách loại hàng hóa vào ViewBag
            ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai", hangHoa.MaLoai);

            // Truyền danh sách nhà cung cấp vào ViewBag
            ViewBag.MaNcc = new SelectList(db.NhaCungCaps.ToList(), "MaNcc", "TenCongTy", hangHoa.MaNcc);

            // Truyền thêm thông tin vào ViewData nếu cần
            ViewData["MaHh"] = hangHoa.MaHh;

            // Trả về view với dữ liệu từ ViewModel
            return View(hanghoa);
        }


        [Route("SuaHangHoa")]
        [HttpPost]
        public IActionResult SuaHangHoa(HangHoaViewModels model, IFormFile Hinh)
        {
            // Kiểm tra nếu có file hình ảnh mới
            if (Hinh != null)
            {
                // Lưu hình ảnh mới và cập nhật trường "Hinh"
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "HangHoa", Hinh.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Hinh.CopyTo(stream);
                }
                model.Hinh = Hinh.FileName; // Cập nhật tên hình ảnh mới vào model
            }
            else
            {
                // Nếu không chọn file mới, giữ nguyên hình ảnh cũ
                var hangHoa = db.HangHoas.Find(model.MaHh);
                model.Hinh = hangHoa?.Hinh; // Giữ lại hình ảnh cũ
            }

            // Cập nhật các trường dữ liệu khác
            var hangHoaToUpdate = db.HangHoas.Find(model.MaHh);
            if (hangHoaToUpdate != null)
            {
                hangHoaToUpdate.TenHh = model.TenHh;
                hangHoaToUpdate.MaLoai = model.MaLoai;
                hangHoaToUpdate.DonGia = model.DonGia;
                hangHoaToUpdate.GiamGia = model.GiamGia;
                hangHoaToUpdate.SoLanXem = model.SoLanXem;
                hangHoaToUpdate.MoTa = model.MoTa;
                hangHoaToUpdate.MaNcc = model.MaNcc;
                hangHoaToUpdate.Hinh = model.Hinh; // Cập nhật hình ảnh
                db.SaveChanges();
            }

            return RedirectToAction("DanhSachHangHoa");
        }

    }
}