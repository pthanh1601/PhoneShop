using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneShop.Data;
using X.PagedList.Extensions;

namespace PhoneShop.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    [Route("admin/NhaCungCaps")]
     [Authorize]
    public class NhaCungCapsController : Controller
    {
        private readonly Hshop2023Context _context;

        public NhaCungCapsController(Hshop2023Context context)
        {
            _context = context;
        }
      
        // GET: Admin/NhaCungCaps
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 10; // Số lượng mục mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1

            // Lấy tất cả nhà cung cấp và phân trang
            var nhacungcap = await _context.NhaCungCaps.ToListAsync();
            var pagedNhacungcap = nhacungcap.ToPagedList(pageNumber, pageSize);

            return View(pagedNhacungcap);
        }
        
        // GET: Admin/NhaCungCaps/Details/5
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhaCungCap = await _context.NhaCungCaps.FirstOrDefaultAsync(m => m.MaNcc == id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }

            return View(nhaCungCap);
        }
       
        // GET: Admin/NhaCungCaps/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
       
        // POST: Admin/NhaCungCaps/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNcc,TenCongTy,Logo,NguoiLienLac,Email,DienThoai,DiaChi,MoTa")] NhaCungCap nhaCungCap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhaCungCap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nhaCungCap);
        }
        
        // GET: Admin/NhaCungCaps/Edit/5
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhaCungCap = await _context.NhaCungCaps.FindAsync(id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }
            return View(nhaCungCap);
        }
        
        // POST: Admin/NhaCungCaps/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaNcc,TenCongTy,Logo,NguoiLienLac,Email,DienThoai,DiaChi,MoTa")] NhaCungCap nhaCungCap)
        {
            if (id != nhaCungCap.MaNcc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhaCungCap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhaCungCapExists(nhaCungCap.MaNcc))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nhaCungCap);
        }
       
        // GET: Admin/NhaCungCaps/Delete/5
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhaCungCap = await _context.NhaCungCaps.FirstOrDefaultAsync(m => m.MaNcc == id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }

            return View(nhaCungCap);
        }
        
        // POST: Admin/NhaCungCaps/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nhaCungCap = await _context.NhaCungCaps.FindAsync(id);
            if (nhaCungCap != null)
            {
                _context.NhaCungCaps.Remove(nhaCungCap);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool NhaCungCapExists(string id)
        {
            return _context.NhaCungCaps.Any(e => e.MaNcc == id);
        }
    }
}
