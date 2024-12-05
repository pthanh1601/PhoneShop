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


    }
}