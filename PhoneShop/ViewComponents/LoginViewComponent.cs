using Microsoft.AspNetCore.Mvc;
using PhoneShop.ViewModels;

namespace PhoneShop.ViewComponents
{
    public class LoginViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var loginVM = new LoginVM(); // Khởi tạo LoginVM nếu cần thiết
            return View(loginVM);
        }
    }
}