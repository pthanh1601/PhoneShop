using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PhoneShop.Data;
using PhoneShop.Helper;
using PhoneShop.ViewModels;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace PhoneShop.Controllers
{
    public class RegistionController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly string secretKey = @"qazwsxedcrfvtgbyhnujmiklopQAZWSXEDCRFVTGBYHNUJMIKLOP!";

        public RegistionController(IConfiguration configuration, Hshop2023Context context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        #region Registion
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(RegisterVM model, IFormFile Hinh)
        {
            // Kiểm tra email đã tồn tại chưa
            if (db.KhachHangs.Any(k => k.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.RandomKey = MyUtil.GenerateRandomKey();
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = false; // Tạm thời chưa kích hoạt
                    khachHang.VaiTro = 0;

                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }

                    db.Add(khachHang);
                    db.SaveChanges();

                    // Tạo ActivationToken với email
                    string activationToken = GenerateActivationToken(khachHang.Email);

                    // Tạo link kích hoạt
                    string activationLink = Url.Action(
                        "ActivateAccount",
                        "Registion",
                        new { token = activationToken },
                        Request.Scheme
                    );

                    // Gửi email kích hoạt
                    SendActivationEmail(khachHang.Email, activationLink);

                    // Chuyển hướng đến View thông báo
                    return RedirectToAction("CheckEmail");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra, vui lòng thử lại.");
                }
            }
            return View(model);
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

        #endregion
        public IActionResult CheckEmail()
        {
            return View();
        }
    }
}
