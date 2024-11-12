using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PhoneShop.Data;

namespace PhoneShop.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Route("admin/NhanVien")]
    public class NhanVienController : Controller
    {
        private readonly Hshop2023Context db;

        public NhanVienController(Hshop2023Context context)
        {
            db = context;
        }

        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await db.NhanViens.ToListAsync());
        }

        // [Admin/NhanVien/AddNhanVien]
        [Route("AddNhanVien")]
        public IActionResult AddNhanVien()
        {
            return View();
        }

        // [Admin/NhanVien/EditNhanVien]
        [Route("EditNhanVien")]
        public IActionResult EditNhanVien()
        {
            return View();
        }

        [Route("DeleteNhanVien")]
        public async Task<IActionResult> DeleteNhanVien(string id)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}
