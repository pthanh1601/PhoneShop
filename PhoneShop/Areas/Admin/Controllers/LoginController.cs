using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneShop.Data;
using PhoneShop.Helper;
using PhoneShop.ViewModels;
using System.Security.Claims;
namespace PhoneShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/Login")]
    public class LoginController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;
        public LoginController(Hshop2023Context context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        #region Login
        [HttpGet]
        [Route("Index")]
        public IActionResult Index(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        [Route("Index")]

        public async Task<IActionResult> Index(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var nhanvien = db.NhanViens.SingleOrDefault(nv => nv.MaNv == model.UserName);
                if (nhanvien == null)
                {
                    ModelState.AddModelError("loi", "Không có tài khoản này");
                }
                else
                {
                    if (nhanvien.MatKhau == model.Password)
                    {
                        ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                    }
                    else
                    {
                        var claims = new List<Claim> {
                                new Claim(ClaimTypes.Email, nhanvien.Email),
                                new Claim(ClaimTypes.Name, nhanvien.HoTen),
                                new Claim(MySetting.CLAIM_CUSTOMERID, nhanvien.MaNv),
                                //claim - role động 
                                new Claim(ClaimTypes.Role, "Admin"),

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
            return View();
        }
        #endregion

    }
}
