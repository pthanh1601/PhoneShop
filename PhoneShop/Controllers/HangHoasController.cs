using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneShop.Data;
using PhoneShop.ViewModels;

namespace PhoneShop.Controllers
{
    public class HangHoasController : Controller
    {
        private readonly Hshop2023Context db;
        public HangHoasController(Hshop2023Context context)
        {
            db = context;
        }

        // Index action để trả về sản phẩm phân trang
        public async Task<IActionResult> Index(int? loai, int page = 1)
        {
            int pageSize = 10; // Số sản phẩm mỗi trang

            // Tạo IQueryable cho danh sách sản phẩm
            var hangHoas = db.HangHoas.AsQueryable();

            // Nếu có tham số lọc theo loại
            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }

            // Chọn các trường dữ liệu cần thiết từ bảng HangHoas
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHh = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MotaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });

            // Sử dụng PaginatedList để phân trang
            var paginatedList = await PaginatedList<HangHoaVM>.CreateAsync(result, page, pageSize);

            return View(paginatedList);
        }

        // Action tìm kiếm sản phẩm (có thể sử dụng với AJAX)
        public IActionResult Search(string? query)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHh = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MotaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });

            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> GetProducts(int? loai, int page = 1)
        {
            int pageSize = 10; // Số sản phẩm mỗi trang

            var hangHoas = db.HangHoas.AsQueryable();

            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }

            // Chọn các trường cần thiết
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHh = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MotaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });

            var paginatedList = await PaginatedList<HangHoaVM>.CreateAsync(result, page, pageSize);

            return Json(new
            {
                data = paginatedList,
                totalPages = paginatedList.TotalPages,
                currentPage = page,
                totalCount = paginatedList.TotalCount
            });
        }
        // Action hiển thị chi tiết sản phẩm
        public IActionResult Detail(int id)
        {
            var data = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);

            if (data == null)
            {
                TempData["Message"] = $"Not found merchandise for id {id}";
                return Redirect("/404");
            }

            var result = new ChiTietHangHoaVM
            {
                MaHh = data.MaHh,
                TenHh = data.TenHh,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh ?? string.Empty,
                MotaNgan = data.MoTaDonVi ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.TenLoai,
                SoLuongTon = 10, // Chưa hoàn chỉnh
                DiemDanhGia = 5, // Chưa hoàn chỉnh
            };

            return View(result);
        }
    }
}
