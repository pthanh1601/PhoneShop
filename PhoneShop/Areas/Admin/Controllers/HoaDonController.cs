using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        // GET: Admin/HoaDon/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            var viewModel = new HoaDonViewModel
            {
                KhachHangs = hshop2023Context.KhachHangs.ToList(),
                HangHoas = hshop2023Context.HangHoas.ToList()
            };
            return View(viewModel);
        }

        // POST: Admin/HoaDon/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HoaDonViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Tạo hóa đơn mới
                var hoaDon = new HoaDon
                {
                    MaKh = viewModel.MaKh,
                    NgayDat = viewModel.NgayDat,
                    NgayCan = viewModel.NgayCan,
                    NgayGiao = viewModel.NgayGiao,
                    HoTen = viewModel.HoTen,
                    DiaChi = viewModel.DiaChi,
                    DienThoai = viewModel.DienThoai,
                    CachThanhToan = viewModel.CachThanhToan,
                    CachVanChuyen = viewModel.CachVanChuyen,
                    PhiVanChuyen = viewModel.PhiVanChuyen,
                    MaTrangThai = viewModel.MaTrangThai,
                    MaNv = viewModel.MaNv?? null,
                    GhiChu = viewModel.GhiChu
                };

                // Thêm hóa đơn vào cơ sở dữ liệu
                hshop2023Context.HoaDons.Add(hoaDon);
                await hshop2023Context.SaveChangesAsync();

                // Thêm chi tiết hóa đơn
                foreach (var chiTiet in viewModel.ChiTietHds)
                {
                    var chiTietHd = new ChiTietHd
                    {
                        MaHd = hoaDon.MaHd,
                        MaHh = chiTiet.MaHh,
                        DonGia = chiTiet.DonGia,
                        SoLuong = chiTiet.SoLuong,
                        GiamGia = chiTiet.GiamGia
                    };
                    hshop2023Context.ChiTietHds.Add(chiTietHd);
                }

                await hshop2023Context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Nếu không hợp lệ, trả về lại form với lỗi
            return View(viewModel);
        }
    }
}