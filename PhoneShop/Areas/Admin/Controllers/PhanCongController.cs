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
            // Kiểm tra tính hợp lệ của dữ liệu
            if (ModelState.IsValid)
            {
                // Gán giá trị mặc định cho NgayPc nếu người dùng không nhập
                var phanCong = new PhanCong
                {
                    MaNv = viewModel.PhanCong.MaNv,
                    MaPb = viewModel.PhanCong.MaPb,
                    NgayPc = viewModel.PhanCong.NgayPc ?? new DateTime(2017, 12, 17, 10, 16, 39, 180),
                    HieuLuc = viewModel.PhanCong.HieuLuc
                };

                // Thêm vào database
                _context.PhanCongs.Add(phanCong);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"New assignment added: {JsonSerializer.Serialize(phanCong)}");

                TempData["SuccessMessage"] = "Thêm phân công thành công.";
                return RedirectToAction("Index");
            }

            // Xuất lỗi của ModelState ra để kiểm tra
            var errorMessages = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            // Ghi log lỗi (nếu có sử dụng logging)
            foreach (var error in errorMessages)
            {
                _logger.LogError($"ModelState Error: {error}");
            }

            // Truyền lỗi vào TempData để hiển thị trên View
            TempData["ModelStateErrors"] = string.Join("; ", errorMessages);

            // Truyền lại các dữ liệu vào viewModel
            viewModel.NhanViens = _context.NhanViens.ToList(); // Lấy danh sách nhân viên từ DB
            viewModel.PhongBans = _context.PhongBans.ToList(); // Lấy danh sách phòng ban từ DB

            // Trả về view cùng dữ liệu
            return View("~/Areas/Admin/Views/HomeAdmin/ThemPhanCong.cshtml", viewModel);
        }


        // GET: Display assignment list
        public async Task<IActionResult> Index()
        {
            var assignments = await _context.PhanCongs.ToListAsync();
            _logger.LogInformation($"Assignments loaded: {JsonSerializer.Serialize(assignments)}");

            return View("~/Areas/Admin/Views/HomeAdmin/PhanCong.cshtml", assignments);
        }

        // Phương thức GET để hiển thị trang sửa phân công
        public IActionResult Edit(int maPc)
        {
            var phanCong = _context.PhanCongs.FirstOrDefault(pc => pc.MaPc == maPc);
            if (phanCong == null)
            {
                return NotFound();
            }

            // Chuyển dữ liệu vào ViewModel
            var viewModel = new PhanCongVM
            {
                PhanCong = phanCong,
                NhanViens = _context.NhanViens.ToList(),
                PhongBans = _context.PhongBans.ToList()
            };

            // Trả về View với ViewModel
            return View("~/Areas/Admin/Views/HomeAdmin/SuaPhanCong.cshtml", viewModel);
        }
        // POST: Update assignment
        [HttpPost]
        public async Task<IActionResult> Update(PhanCongVM model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu ModelState không hợp lệ, trả về lại trang sửa với dữ liệu hiện tại
                TempData["ModelStateErrors"] = string.Join(";", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                model.NhanViens = _context.NhanViens.ToList();
                model.PhongBans = _context.PhongBans.ToList();
                return View("~/Areas/Admin/Views/HomeAdmin/SuaPhanCong.cshtml", model);
            }

            // Tìm phân công trong cơ sở dữ liệu dựa trên MaPc
            var existingPhanCong = await _context.PhanCongs.FindAsync(model.PhanCong.MaPc);
            if (existingPhanCong == null)
            {
                TempData["ErrorMessage"] = "Phân công không tồn tại.";
                return RedirectToAction("Index");
            }

            // Cập nhật dữ liệu phân công từ model
            existingPhanCong.MaNv = model.PhanCong.MaNv;
            existingPhanCong.MaPb = model.PhanCong.MaPb;
            existingPhanCong.NgayPc = model.PhanCong.NgayPc;
            existingPhanCong.HieuLuc = model.PhanCong.HieuLuc;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.Update(existingPhanCong);
            await _context.SaveChangesAsync();

            // Thông báo thành công và chuyển hướng về trang danh sách phân công
            TempData["SuccessMessage"] = "Cập nhật phân công thành công.";
            return RedirectToAction("Index");
        }
        // POST: Delete assignment
        [HttpPost]
        public async Task<IActionResult> Delete(int maPc)
        {
            // Tìm phân công cần xóa từ cơ sở dữ liệu
            var phanCong = await _context.PhanCongs
                .FirstOrDefaultAsync(pc => pc.MaPc == maPc);

            // Nếu không tìm thấy phân công, trả về lỗi 404
            if (phanCong == null)
            {
                TempData["ErrorMessage"] = "Phân công không tồn tại.";
                return RedirectToAction("Index");
            }

            // Xóa phân công
            _context.PhanCongs.Remove(phanCong);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Phân công với MaPc {maPc} đã bị xóa.");

            TempData["SuccessMessage"] = "Xóa phân công thành công.";
            return RedirectToAction("Index");
        }

    }
}
