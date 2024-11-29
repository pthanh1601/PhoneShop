using System.Net.Mail;
using System.Net;
using System.Text;
using System.Security.Cryptography;

namespace PhoneShop.Helper
{
    public class MyUtil
    {
        public static string UploadHinh(IFormFile Hinh, string folder)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                    "Hinh", folder, Hinh.FileName);
                using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
                {
                    Hinh.CopyTo(myfile);
                }
                return Hinh.FileName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string GenerateRandomKey(int length = 5)
        {
            var pattern = @"qazwsxedcrfvtgbyhnujmiklopQAZWSXEDCRFVTGBYHNUJMIKLOP!";
            var sb = new StringBuilder();
            var rd = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }

            return sb.ToString();
        }
        public static void SendEmail(IConfiguration configuration, string toEmail, string subject, string body)
        {
            try
            {
                // Lấy thông tin từ appsettings
                var emailSettings = configuration.GetSection("EmailSettings");
                string smtpServer = emailSettings["SMTPServer"];
                int smtpPort = int.Parse(emailSettings["SMTPPort"]);
                string senderEmail = emailSettings["SenderEmail"];
                string senderPassword = emailSettings["SenderPassword"];

                // Cấu hình thông tin SMTP
                var smtpClient = new SmtpClient(smtpServer) // Thay bằng SMTP server bạn dùng
                {
                    Port = smtpPort, // Thông thường là 587 cho TLS
                    Credentials = new NetworkCredential(senderEmail, senderPassword), // Email và mật khẩu
                    EnableSsl = true // Sử dụng SSL
                };

                // Tạo email
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail), // Email gửi đi
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Cho phép HTML trong email
                };

                mailMessage.To.Add(toEmail); // Email người nhận

                // Gửi email
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi gửi email (nếu cần)
                throw new Exception($"Không thể gửi email: {ex.Message}", ex);
            }
        }
    }
}
