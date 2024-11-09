using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhoneShop.Data;
using PhoneShop.ViewModels;
using X.PagedList;
using X.PagedList.Extensions;

namespace MyApp.Namespace
{
    [Area("Admin")]
    [Route("Admin/HoaDon")]

    public class HoaDonController : Controller
    {
        private readonly Hshop2023Context hshop2023Context;
        public HoaDonController(Hshop2023Context context)
        {
            hshop2023Context = context;
        }
        // GET: Admin/HoaDon
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 10; // Số lượng mục mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1

            var hoaDons = await hshop2023Context.HoaDons
    .Include(h => h.MaKhNavigation)         // Bao gồm thông tin KhachHang
    .Include(h => h.MaNvNavigation)         // Bao gồm thông tin NhanVien
    .Include(h => h.MaTrangThaiNavigation)  // Bao gồm thông tin TrangThai
    .ToListAsync();

            var pagedHoaDons = hoaDons.ToPagedList(pageNumber, pageSize);

            return View(pagedHoaDons);
        }

        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var hoaDon = await hshop2023Context.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaNvNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .Include(h => h.ChiTietHds)
                    .ThenInclude(c => c.MaHhNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            ViewBag.TrangThaiList = hshop2023Context.TrangThais.ToList();
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int MaHd, int MaTrangThai)
        {
            var hoaDon = hshop2023Context.HoaDons.Find(MaHd);
            if (hoaDon != null)
            {
                hoaDon.MaTrangThai = MaTrangThai;
                hshop2023Context.SaveChanges();
            }
            return RedirectToAction("Details", new { id = MaHd });
        }

        // Thêm hoá đơn
        [Route("Create")]
        public IActionResult Create()
        {
            // Lấy danh sách khách hàng, trạng thái và mặt hàng để hiển thị trong form
            ViewBag.KhachHang = new SelectList(hshop2023Context.KhachHangs, "MaKh", "HoTen");
            ViewBag.TrangThai = new SelectList(hshop2023Context.TrangThais, "MaTrangThai", "MaTrangThai");
            ViewBag.HangHoa = new SelectList(hshop2023Context.HangHoas, "MaHh", "TenHh");

            return View();
        }


        // POST: Thêm hóa đơn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKh, NgayDat, NgayCan, NgayGiao, HoTen, DiaChi, DienThoai, CachThanhToan, CachVanChuyen, PhiVanChuyen, MaTrangThai, MaNv, GhiChu, ChiTietHds")] HoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                // Lưu hóa đơn mới vào cơ sở dữ liệu
                hshop2023Context.Add(hoaDon);
                await hshop2023Context.SaveChangesAsync();

                // Sau khi lưu hóa đơn, thêm chi tiết hóa đơn vào cơ sở dữ liệu
                foreach (var chiTiet in hoaDon.ChiTietHds)
                {
                    chiTiet.MaHd = hoaDon.MaHd;  // Gán MaHd cho chi tiết hóa đơn
                    hshop2023Context.Add(chiTiet);
                }

                // Lưu lại các chi tiết hóa đơn
                await hshop2023Context.SaveChangesAsync();

                // Chuyển hướng về trang danh sách hóa đơn sau khi thêm thành công
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, hiển thị lại form và dữ liệu đã nhập
            ViewBag.KhachHang = new SelectList(hshop2023Context.KhachHangs, "MaKh", "HoTen", hoaDon.MaKh);
            ViewBag.TrangThai = new SelectList(hshop2023Context.TrangThais, "MaTrangThai", "MaTrangThai", hoaDon.MaTrangThai);
            ViewBag.HangHoa = new SelectList(hshop2023Context.HangHoas, "MaHh", "TenHh");

            return View(hoaDon);
        }


    }
}