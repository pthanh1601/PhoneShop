using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneShop.Data;
using PhoneShop.Helper;
using PhoneShop.ViewModels;
using System.Security.Claims;
namespace PhoneShop.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;
        public KhachHangController(Hshop2023Context context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        #region Register
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DangKy(RegisterVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.RandomKey = MyUtil.GenerateRandomKey();
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true;// xử lý khi dùng mail để active
                    khachHang.VaiTro = 0;

                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }

                    db.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("Index", "HangHoa");
                }
                catch (Exception ex)
                {

                }
            }
            return View();
        }
        #endregion



        #region Login
        [HttpGet]
        public IActionResult DangNhap(string ? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if(ModelState.IsValid){
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
                if(khachHang == null)
                {
                    ModelState.AddModelError("loi", "Không có khách hàng này");
                }
                else
                {
                    if (!khachHang.HieuLuc)
                    {
                        ModelState.AddModelError("loi", "Tài khoản đã bị khóa. Vui lòng liên hệ Admin.");
                    }
                    else
                    {
                        if (khachHang.MatKhau == model.Password.ToMd5Hash(khachHang.RandomKey))
                        {
                            ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                        }
                        else
                        {
                            var claims = new List<Claim> {
                                new Claim(ClaimTypes.Email, khachHang.Email),
                                new Claim(ClaimTypes.Name, khachHang.HoTen),
                                new Claim(MySetting.CLAIM_CUSTOMERID, khachHang.MaKh),
                                //claim - role động 
                                new Claim(ClaimTypes.Role, "Customer"),

                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);


                            await HttpContext.SignInAsync(claimsPrincipal);

                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return Redirect("/");
                            }
                        }
                    }
                }
            }
            return View();
        }
        #endregion

        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        #region Danh Sách Khách Hàng
        [Authorize] // Optionally, you can secure this action if needed
        public IActionResult DanhSachKhachHang()
        {
            var danhSachKhachHang = db.KhachHangs.ToList();
            return View(danhSachKhachHang);
        }
        #endregion
        #region Create KhachHang
        [HttpGet]
        public IActionResult ThemKhachHang()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ThemKhachHang(RegisterVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = new KhachHang
                    {
                        MaKh = model.MaKh,
                        HoTen = model.HoTen,
                        MatKhau = model.MatKhau.ToMd5Hash(MyUtil.GenerateRandomKey()), // Hash the password
                        Email = model.Email,
                        DiaChi = model.DiaChi,
                        DienThoai = model.DienThoai,
                        GioiTinh = model.GioiTinh,
                        NgaySinh = (DateTime)model.NgaySinh,
                        HieuLuc = true,
                        VaiTro = 0
                    };

                    // Handle image upload if Hinh is provided
                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }

                    db.KhachHangs.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("DanhSachKhachHang"); // Redirect to the list view
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while adding the customer: " + ex.Message);
                }
            }

            return View(model); // Return the view with the current model
        }
        #endregion

        #region Edit KhachHang
        [HttpGet]
        public IActionResult SuaKhachHang(string id)
        {
            var khachHang = db.KhachHangs.Find(id);
            if (khachHang == null) return NotFound();

            return View(khachHang);
        }

        [HttpPost]
        public IActionResult SuaKhachHang(KhachHang model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.Find(model.MaKh);
                if (khachHang == null) return NotFound();

                // Update the properties
                khachHang.HoTen = model.HoTen;
                khachHang.Email = model.Email;
                khachHang.DiaChi = model.DiaChi;
                khachHang.DienThoai = model.DienThoai;
                khachHang.GioiTinh = model.GioiTinh;
                khachHang.NgaySinh = model.NgaySinh;

                // Handle image upload if Hinh is provided
                if (Hinh != null)
                {
                    khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                }

                db.SaveChanges();
                return RedirectToAction("DanhSachKhachHang");
            }

            return View(model); // Return the view with the current model
        }
        #endregion

        #region Delete KhachHang
        [HttpGet]
        public IActionResult XoaKhachHang(string id)
        {
            var khachHang = db.KhachHangs.Find(id);
            if (khachHang == null) return NotFound();
            return View(khachHang);
        }

        [HttpPost, ActionName("XoaKhachHang")]
        public IActionResult XacNhanXoa(string id)
        {
            var khachHang = db.KhachHangs.Find(id);
            if (khachHang != null)
            {
                db.KhachHangs.Remove(khachHang);
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachKhachHang");
        }
        #endregion
    }
}
