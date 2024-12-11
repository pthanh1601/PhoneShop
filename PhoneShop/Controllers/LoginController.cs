using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PhoneShop.Data;
using PhoneShop.Helper;
using PhoneShop.ViewModels;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;

namespace PhoneShop.Controllers
{
    public class LoginController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IConfiguration _configuration;
        private readonly string secretKey = @"qazwsxedcrfvtgbyhnujmiklopQAZWSXEDCRFVTGBYHNUJMIKLOP!";

        public LoginController(Hshop2023Context context, IMapper mapper)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Index(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.Email == model.Email);
                if (khachHang != null)
                {
                    if (khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
                    {
                        ModelState.AddModelError(nameof(model.Password), "Sai mật khẩu.");
                    }
                    else if (!khachHang.HieuLuc)
                    {
                        ViewBag.EmailNotActivated = true;
                        ViewBag.UserEmail = khachHang.Email;
                        return View(model);
                    }
                    else
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, khachHang.Email),
                            new Claim(ClaimTypes.Name, khachHang.HoTen),
                            new Claim(MySetting.CLAIM_CUSTOMERID, khachHang.MaKh),
                            new Claim(ClaimTypes.Role, "Customer"),
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(claimPrincipal);

                        return Redirect(ReturnUrl ?? "/");
                    }
                }
                else
                {
                    var nhanVien = db.NhanViens.SingleOrDefault(nv => nv.Email == model.Email);
                    if (nhanVien != null)
                    {
                        if (nhanVien.MatKhau != model.Password)
                        {
                            ModelState.AddModelError(nameof(model.Password), "Sai mật khẩu.");
                        }
                        else
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, nhanVien.Email),
                                new Claim(ClaimTypes.Name, nhanVien.HoTen),
                                new Claim(MySetting.CLAIM_EMPLOYEERID, nhanVien.MaNv),
                                new Claim(ClaimTypes.Role, "Employee"),
                            };
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(claimPrincipal);
                            return Redirect(ReturnUrl ?? "/Admin/");
                        }
                    }
                    else
                    {
                        // Nếu không tìm thấy trong cả hai bảng
                        ModelState.AddModelError(nameof(model.Email), "Tài khoản không tồn tại.");
                    }
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginAjax(LoginVM model, string? ReturnUrl)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.Email == model.Email);
                if (khachHang != null)
                {
                    Console.WriteLine(model.Password);

                    Console.WriteLine($"KhachHang Password: {khachHang.MatKhau}");
                    Console.WriteLine($"Model Password: {model.Password.ToMd5Hash(khachHang.RandomKey)}");
                    if (khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
                    {
                        ModelState.AddModelError(nameof(model.Password), "Sai mật khẩu.");
                    }

                    if (!khachHang.HieuLuc)
                    {
                        return Json(new { success = false, message = "Tài khoản chưa được kích hoạt." });
                    }
                    else
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, khachHang.Email),
                    new Claim(ClaimTypes.Name, khachHang.HoTen),
                    new Claim(MySetting.CLAIM_CUSTOMERID, khachHang.MaKh),
                    new Claim(ClaimTypes.Role, "Customer"),
                };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimPrincipal);

                        return Json(new { success = true, redirectUrl = ReturnUrl ?? "/" });
                    }
                }
                else
                {
                    var nhanVien = db.NhanViens.SingleOrDefault(nv => nv.Email == model.Email);
                    if (nhanVien != null)
                    {
                        if (nhanVien.MatKhau != model.Password)
                        {
                            return Json(new { success = false, message = "Sai mật khẩu." });
                        }
                        else
                        {
                            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, nhanVien.Email),
                    new Claim(ClaimTypes.Name, nhanVien.HoTen),
                    new Claim(MySetting.CLAIM_EMPLOYEERID, nhanVien.MaNv),
                    new Claim(ClaimTypes.Role, "Employee"),
                };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(claimPrincipal);

                            return Json(new { success = true, redirectUrl = ReturnUrl ?? "/Admin/" });
                        }

                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.Email), "Tài khoản không tồn tại.");
                    }
                }
            }


            // Trả về lỗi cho từng trường nếu có
            var errors = ModelState.Where(m => m.Value.Errors.Any())
                                   .ToDictionary(
                                       m => m.Key,
                                       m => m.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                   );
            return Json(new { success = false, errors });
        }

        // Action gửi lại email kích hoạt
        [HttpPost]
        public IActionResult ResendActivationEmail(string email)
        {
            var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.Email == email && !kh.HieuLuc);

            if (khachHang != null)
            {
                // Logic gửi email kích hoạt
                // Thay "SendActivationEmail" bằng phương thức thực tế của bạn
                SendActivationEmail(khachHang.Email, khachHang.RandomKey);
                TempData["SuccessMessage"] = "Email kích hoạt đã được gửi lại. Vui lòng kiểm tra hộp thư.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản chưa kích hoạt.";
            }

            return RedirectToAction("Index");
        }


        // Hàm kích hoạt tài khoản
        [HttpGet]
        public async Task<IActionResult> ActivateAccount(string token)
        {
            // Giải mã token và kiểm tra tính hợp lệ
            string email = ValidateActivationToken(token);

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Token không hợp lệ hoặc đã hết hạn.");
            }

            // Lấy khách hàng từ cơ sở dữ liệu
            var khachHang = db.KhachHangs.FirstOrDefault(k => k.Email == email);
            if (khachHang == null)
            {
                return BadRequest("Không tìm thấy khách hàng.");
            }

            // Cập nhật trạng thái kích hoạt
            khachHang.HieuLuc = true;
            db.SaveChanges();

            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, khachHang.Email),
                            new Claim(ClaimTypes.Name, khachHang.HoTen),
                            new Claim(MySetting.CLAIM_CUSTOMERID, khachHang.MaKh),
                            new Claim(ClaimTypes.Role, "Customer"),
                        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal);

            return RedirectToAction("Index", "/");

        }

        // Tạo ActivationToken (JWT hoặc mã hóa)
        private string GenerateActivationToken(string email)
        {
            var payload = $"{email}|{DateTime.UtcNow.AddMinutes(10):O}";
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{payload}|{secretKey}"));
            return token;
        }

        // Kiểm tra ActivationToken
        private string ValidateActivationToken(string token)
        {
            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var parts = decoded.Split('|');

                if (parts.Length != 3)
                    return null;

                var email = parts[0];
                var expiration = DateTime.Parse(parts[1]);
                var key = parts[2];

                if (expiration < DateTime.UtcNow || key != secretKey)
                    return null;

                return email;
            }
            catch
            {
                return null;
            }
        }

        // Gửi email kích hoạt
        private void SendActivationEmail(string email, string activationLink)
        {
            // Sử dụng dịch vụ gửi email (ví dụ: SMTP)
            // Code dưới đây là một ví dụ đơn giản
            var subject = "Kích hoạt tài khoản của bạn";
            var body = $@"
        <h1>Chào mừng bạn đến với PhoneShop!</h1>
        <p>Vui lòng nhấn vào liên kết bên dưới để kích hoạt tài khoản của bạn:</p>
        <a href='{HtmlEncoder.Default.Encode(activationLink)}'>Kích hoạt tài khoản</a>
        <p>Nếu bạn không yêu cầu tài khoản này, hãy bỏ qua email này.</p>
        <p>Thân mến,<br/>Đội ngũ PhoneShop</p>";

            MyUtil.SendEmail(_configuration, email, subject, body);
        }
    }
}
