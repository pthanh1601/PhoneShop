using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneShop.Data;
using PhoneShop.ViewModels;
using Microsoft.Extensions.Logging;
using System.Text.Json; // Namespace for JSON serialization

namespace PhoneShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhanCongController : Controller
    {
        private readonly Hshop2023Context _context;
        private readonly ILogger<PhanCongController> _logger;

        public PhanCongController(Hshop2023Context context, ILogger<PhanCongController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Display Create Assignment page
        public IActionResult Create()
        {
            var model = new PhanCongVM
            {
                PhanCong = new PhanCong(),
                NhanViens = _context.NhanViens.ToList(),
                PhongBans = _context.PhongBans.ToList()
            };

            return View("~/Areas/Admin/Views/HomeAdmin/ThemPhanCong.cshtml", model);
        }

        // POST: Add assignment to the database
        [HttpPost]
        public async Task<IActionResult> Add(PhanCongVM viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var phanCong = new PhanCong
                    {
                        MaNv = viewModel.PhanCong.MaNv,
                        MaPb = viewModel.PhanCong.MaPb,
                        NgayPc = viewModel.PhanCong.NgayPc,
                        HieuLuc = viewModel.PhanCong.HieuLuc
                    };

                    _context.PhanCongs.Add(phanCong);
                    await _context.SaveChangesAsync();

                    // Log JSON serialized data to debug console
                    _logger.LogInformation($"New assignment added: {JsonSerializer.Serialize(phanCong)}");

                    TempData["SuccessMessage"] = "Thêm phân công thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error adding assignment: {ex.Message}");
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm phân công. Vui lòng thử lại.");

                    viewModel.NhanViens = _context.NhanViens.ToList();
                    viewModel.PhongBans = _context.PhongBans.ToList();
                    return View("~/Areas/Admin/Views/HomeAdmin/ThemPhanCong.cshtml", viewModel);
                }
            }

            viewModel.NhanViens = _context.NhanViens.ToList();
            viewModel.PhongBans = _context.PhongBans.ToList();
            return View("~/Areas/Admin/Views/HomeAdmin/ThemPhanCong.cshtml", viewModel);
        }

        // GET: Display assignment list
        public async Task<IActionResult> Index()
        {
            var assignments = await _context.PhanCongs.ToListAsync();
            _logger.LogInformation($"Assignments loaded: {JsonSerializer.Serialize(assignments)}");

            return View("~/Areas/Admin/Views/HomeAdmin/PhanCong.cshtml", assignments);
        }

        // GET: Display Edit Assignment page
        public IActionResult Edit(int maPc)
        {
            var phanCong = _context.PhanCongs.FirstOrDefault(pc => pc.MaPc == maPc);
            if (phanCong == null)
            {
                return NotFound();
            }

            var viewModel = new PhanCongVM
            {
                PhanCong = phanCong,
                NhanViens = _context.NhanViens.ToList(),
                PhongBans = _context.PhongBans.ToList()
            };

            return View("~/Areas/Admin/Views/HomeAdmin/SuaPhanCong.cshtml", viewModel);
        }

        // POST: Update assignment
        [HttpPost]
        public async Task<IActionResult> Update(PhanCongVM viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingPhanCong = await _context.PhanCongs.FirstOrDefaultAsync(pc => pc.MaPc == viewModel.PhanCong.MaPc);
                    if (existingPhanCong != null)
                    {
                        existingPhanCong.MaNv = viewModel.PhanCong.MaNv;
                        existingPhanCong.MaPb = viewModel.PhanCong.MaPb;
                        existingPhanCong.NgayPc = viewModel.PhanCong.NgayPc;
                        existingPhanCong.HieuLuc = viewModel.PhanCong.HieuLuc;

                        _context.PhanCongs.Update(existingPhanCong);
                        await _context.SaveChangesAsync();

                        // Log JSON serialized data to debug console
                        _logger.LogInformation($"Assignment updated: {JsonSerializer.Serialize(existingPhanCong)}");

                        TempData["SuccessMessage"] = "Cập nhật phân công thành công.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error updating assignment: {ex.Message}");
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật phân công.");

                    viewModel.NhanViens = _context.NhanViens.ToList();
                    viewModel.PhongBans = _context.PhongBans.ToList();
                    return View("~/Areas/Admin/Views/HomeAdmin/SuaPhanCong.cshtml", viewModel);
                }
            }

            viewModel.NhanViens = _context.NhanViens.ToList();
            viewModel.PhongBans = _context.PhongBans.ToList();
            return View("~/Areas/Admin/Views/HomeAdmin/SuaPhanCong.cshtml", viewModel);
        }

        // POST: Delete assignment
        [HttpPost]
        public async Task<IActionResult> Delete(int maPc)
        {
            try
            {
                var phanCong = await _context.PhanCongs.FirstOrDefaultAsync(pc => pc.MaPc == maPc);
                if (phanCong != null)
                {
                    _context.PhanCongs.Remove(phanCong);
                    await _context.SaveChangesAsync();

                    // Log JSON serialized data to debug console
                    _logger.LogInformation($"Assignment deleted: {JsonSerializer.Serialize(phanCong)}");

                    TempData["SuccessMessage"] = "Xóa phân công thành công.";
                    return RedirectToAction(nameof(Index));
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting assignment: {ex.Message}");
                return BadRequest("Có lỗi xảy ra khi xóa phân công.");
            }
        }
    }
}
